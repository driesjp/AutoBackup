using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using System.Collections.Generic;

namespace AutoBackup
{
    // service to handle file operations
    public class Worker : BackgroundService
    {
        private ConcurrentQueue<IFileTask> fileTaskQueue = new ConcurrentQueue<IFileTask>();
        private string backupBasePath; // target base path for copied files, to be loaded from config file
        private List<string> pathsToMonitor = new List<string>(); // source paths to monitor, to be loaded from config file
        private ConcurrentDictionary<string, (DateTime FirstEventTime, DateTime LastWriteTime, long LastSize, int EventCount, DateTime LastProcessed)> fileEventTrackerDict = new ConcurrentDictionary<string, (DateTime, DateTime, long, int, DateTime)>();


        public Worker()
        {
            LoadConfig();   // load XML config data
        }

        private void LoadConfig()
        {
            // load XML config file
            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Settings.xml"); // settings xml file

            // load 'target base path for copied files' from config
            backupBasePath = doc.Root.Element("TargetFolder").Value + @"\";

            // load 'source paths to monitor' from config
            var folderSources = doc.Root.Element("FolderSources").Elements("string");
            foreach (var folder in folderSources)
            {
                pathsToMonitor.Add(folder.Value);
            }
        }
        // method to execute the service
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // start file checker in a separate thread
            Task fileCheckerTask = Task.Run(() => FileChecker(stoppingToken), stoppingToken);
            // create a list of FileSystemWatchers to monitor the paths
            List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
            
            // create a FileSystemWatcher for each path to monitor
            foreach (var path in pathsToMonitor)
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = path,
                    NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName,
                    IncludeSubdirectories = true
                };

                // attach event handlers for file changes
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnDeleted;
                watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;

                watchers.Add(watcher);
            }

            // start file processing task
            Task fileProcessingTask = Task.Run(() => ProcessFileTasks(stoppingToken), stoppingToken);

            // wait for the service to be stopped
            await WaitWhileServiceIsRunning(stoppingToken);

            // dispose of all the watchers when service stops
            foreach (var watcher in watchers)
            {
                watcher.Dispose();
            }

            // wait for the file processing and event checker tasks to complete
            await Task.WhenAll(fileProcessingTask, fileCheckerTask);
        }

        // method to compose the correct path for backed up files
        private string targetFile(string fullPath, string fileName)
        {
            string driveLetter = Path.GetPathRoot(fullPath);
            string directoryPath = Path.GetDirectoryName(fullPath);
            string targetToPass;

            if (directoryPath != null && directoryPath.StartsWith(driveLetter)) directoryPath = directoryPath.Substring(driveLetter.Length);
            int lastBackslashIndex = fileName.LastIndexOf(@"\");   // prepare extract filename.extension
            targetToPass = backupBasePath + driveLetter.Remove(1) + @"\" + directoryPath + @"\" + fileName;

            return targetToPass;
        }

        // event handlers for file copying tasks
        private async void OnChanged(object source, FileSystemEventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            // avoid duplicate tasks for the same file

            // lock the dictionary to prevent concurrent access
            lock (fileEventTrackerDict)
            {
                // check if an entry exists for the file
                if (fileEventTrackerDict.TryGetValue(e.FullPath, out var fileInfo))
                {
                    if (currentTime - fileInfo.LastProcessed < TimeSpan.FromSeconds(2))
                    {
                        return; 
                    }
                }
                else
                {
                    // if there is not information on the file, create dictionary entry
                    fileInfo = (currentTime, DateTime.MinValue, -1, 0, DateTime.MinValue);
                    fileEventTrackerDict[e.FullPath] = fileInfo;
                }
            }

            // delay to wait for stabilization of file events (ignore rapid events)
            await Task.Delay(2000);

            // lock the dictionary to prevent concurrent access, recheck
            lock (fileEventTrackerDict)
            {
                // check the latest state of the file
                if (fileEventTrackerDict.TryGetValue(e.FullPath, out var updatedInfo))
                {
                    // file doesn't exist or was recently processed
                    FileInfo fileInfo = new FileInfo(e.FullPath);
                    if (!fileInfo.Exists || currentTime - updatedInfo.LastProcessed < TimeSpan.FromSeconds(2))
                    {
                        return; 
                    }

                    DateTime lastWriteTime = fileInfo.LastWriteTime;
                    long size = fileInfo.Length;

                    // update dictionary
                    fileEventTrackerDict[e.FullPath] = (updatedInfo.FirstEventTime, lastWriteTime, size, updatedInfo.EventCount + 1, currentTime);

                    // enqueue task for processing
                    fileTaskQueue.Enqueue(new FileCopyTask(e.FullPath, targetFile(e.FullPath, e.Name)));
                }
            }
        }

        private void FileChecker(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // cleanup mechanism to remove old entries from the dictionary
                foreach (var kvp in fileEventTrackerDict)
                {
                    if (DateTime.Now - kvp.Value.LastWriteTime > TimeSpan.FromMinutes(30)) // if files are older than 30 minutes
                    {
                        fileEventTrackerDict.TryRemove(kvp.Key, out _); // remove them from the dictionary
                    }
                }
                Thread.Sleep(10000); // Sleep for 10 seconds
            }
        }

        // event handlers for file deleting tasks
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                // add delete task to the queue for processing
                fileTaskQueue.Enqueue(new FileDeleteTask(targetFile(e.FullPath, e.Name)));
            }
            catch (Exception ex)
            {
                Logger.Instance.Log($"Error enqueuing delete task for {e.FullPath}: {ex.Message}");
            }
        }

        // event handlers for file renaming tasks
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            try
            {
                // add rename task to the queue for processing
                fileTaskQueue.Enqueue(new FileRenameTask(targetFile(e.OldFullPath, e.OldName), targetFile(e.FullPath, e.Name)));
            }
            catch (Exception ex)
            {
                Logger.Instance.Log($"Error enqueuing rename task for {e.OldFullPath} to {e.FullPath}: {ex.Message}");
            }
        }

        // check periodically if the service is running
        private async Task WaitWhileServiceIsRunning(CancellationToken stoppingToken)
        {
            // when stop stoken is set, stop the loop and exit method
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        // method to process the file tasks
        private void ProcessFileTasks(CancellationToken stoppingToken)
        {
            // loop until the service is stopped
            while (!stoppingToken.IsCancellationRequested)
            {
                // dequeue a task and execute it
                if (fileTaskQueue.TryDequeue(out IFileTask task))
                {
                    task.Execute();
                }
                else
                {
                    // no tasks in the queue, delay a bit. set to 1 second to go easy on the cpu. allow to adjust in future versions?
                    Task.Delay(1000).Wait();
                }
            }
        }
    }
}