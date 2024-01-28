using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{
    // task: delete file

    public class FileDeleteTask : IFileTask
    {
        private string SourcePath { get; }

        public FileDeleteTask(string sourcePath)
        {
            SourcePath = sourcePath;
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
                    // delete the directory, true flag is to delete recursively
                    Directory.Delete(SourcePath, true);
                    Logger.Instance.Log($"Deleted directory '{SourcePath}'");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log($"Error deleting directory: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    // delete the file
                    File.Delete(SourcePath);
                    Logger.Instance.Log($"Deleted '{SourcePath}'");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log($"Error deleting file: {ex.Message}");
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