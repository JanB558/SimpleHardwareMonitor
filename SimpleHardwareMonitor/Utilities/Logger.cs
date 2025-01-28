using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Utilities
{
    public static class Logger
    {
        private static readonly object _lockObject = new();
        private static readonly string _logDirectory = 
            AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _logFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ErrorLog.log");

        public static void LogError(string errorMessage)
        {
            lock (_lockObject)
            {
                try
                {
                    EnsureDirectoryExists();

                    using StreamWriter writer = new(_logFilePath, true);
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    writer.WriteLine($"[{timestamp}] ERROR: {errorMessage}");
                }
                catch { }
            }
        }

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }
    }
}
