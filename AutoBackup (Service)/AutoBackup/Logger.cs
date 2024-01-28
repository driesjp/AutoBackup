using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{
    // logger class (singleton)
    public class Logger
    {
        private static Logger instance = null;
        private static readonly object padlock = new object();
        private readonly string logFilePath;

        private Logger()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            logFilePath = Path.Combine(logDirectory, "log.txt");
        }

        public static Logger Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null) instance = new Logger();
                    return instance;
                }
            }
        }

        public void Log(string message)
        {
            try
            {
                // prepend the date and time to each log entry
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Placeholder for something amazing?
            }
        }
    }
}