using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{
    // task: rename file

    public class FileRenameTask : IFileTask
    {
        private string SourcePath { get; }
        private string DestinationPath { get; }

        public FileRenameTask(string sourcePath, string destinationPath)
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
            // check if the source is a directory
            if (Directory.Exists(SourcePath))
            {
                try
                {
                    // rename the directory
                    Directory.Move(SourcePath, DestinationPath);
                    Logger.Instance.Log($"Renamed directory '{SourcePath}' to '{DestinationPath}'");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log($"Error renaming directory: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    // rename the file
                    File.Move(SourcePath, DestinationPath);
                    Logger.Instance.Log($"Renamed '{SourcePath}' to '{DestinationPath}'");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log($"Error renaming file: {ex.Message}. This is common for files written in fast, consecutive stages.");
                }
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