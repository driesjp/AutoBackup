using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{

    // task: copy file

    public class FileCopyTask : IFileTask
    {
        private string SourcePath { get; }
        private string DestinationPath { get; }

        public FileCopyTask(string sourcePath, string destinationPath)
        {
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
        }

        public void Execute()
        {
            if (IsTemporary(SourcePath))
            {
                Logger.Instance.Log($"Skipping temporary file or directory: '{SourcePath}'");
                return;
            }
            try
            {
                // check if the source is a directory
                if (Directory.Exists(SourcePath))
                {
                    // it's a directory, so handle directory copy
                    CopyDirectory(SourcePath, DestinationPath);
                    Logger.Instance.Log($"Copied directory '{SourcePath}' to '{DestinationPath}'");
                }
                else
                {
                    // it's a file, so do file copy
                    // create the directory for the destination path if it does not exist
                    string destinationDirectory = Path.GetDirectoryName(DestinationPath);
                    Directory.CreateDirectory(destinationDirectory);

                    File.Copy(SourcePath, DestinationPath, overwrite: true);
                    Logger.Instance.Log($"Copied file '{SourcePath}' to '{DestinationPath}'");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log($"Error during copy: {ex.Message}");
            }
        }

        // method called from FileCopyTask to copy a directory
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destinationDir));
            }

            // copy all the files & overwrite existing ones
            foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceDir, destinationDir), true);
            }
        }
        private bool IsTemporary(string path)
        {
            // Define logic to determine if the path is a temporary file or directory
            string fileName = Path.GetFileName(path);
            return fileName.StartsWith("~$") || Path.GetExtension(path).Equals(".tmp", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".TMP", StringComparison.OrdinalIgnoreCase);
        }
    }
}