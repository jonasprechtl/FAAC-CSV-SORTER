using System.Reflection;
using Microsoft.Win32;

namespace Log
{
    public class Logger
    {

        private static string logFile;
        private static LogLevel logLevel = LogLevel.Info;

        public static void InitializeLogger()
        {
            logFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ServiceLog.log");
            Log("---- FIRST LOG MESSAGE FOR THIS RUN ----", LogLevel.Info);

            setLogLevel();

            GeneralLogData.LogSystemInfo();
            GeneralLogData.LogCurrentConfig();

            Log("Logger initialized", LogLevel.Info);
            Log("Logfile Path: " + logFile, LogLevel.Info);

            //try to get the key "LOGLEVEL" from the registry 
        }

        public static void setLogLevel()
        {
            try
            {
                string key = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\FAAC\CSVFixer", "LOGLEVEL", "1").ToString();
                if (Enum.TryParse(key, out LogLevel level))
                {
                    logLevel = level;
                    Log("Loglevel set to " + logLevel, LogLevel.Info);
                }
                else
                {
                    logLevel = LogLevel.Info;
                    Log("Loglevel could not be parsed, using INFO Level as default", LogLevel.Warning);
                }

            }
            catch (Exception e)
            {
                logLevel = LogLevel.Info;
                Log("Error while trying to read the registry key for LogLevel: " + e.Message, LogLevel.Error);
                Log("Using INFO Level as default", LogLevel.Info);
            }

        }

        public static void Log(string message, LogLevel level)
        {
            if (level < logLevel)
            {
                return;
            }
            if (string.IsNullOrEmpty(logFile))
            {
                //No logmessage before the logger was initialized, would crash the program either because file not defined or infinite loop
                InitializeLogger();
            }

            // Log the message to a file and the Console.
            string purifiedMessage = purifyLogMessage(message, level);
            Console.WriteLine(purifiedMessage);
            File.AppendAllText(logFile, purifiedMessage + Environment.NewLine);
        }

        private static string purifyLogMessage(string message, LogLevel level)
        {
            //Add the timestamp to the message
            message = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] <{level}> - {message}";
            return message;
        }
    }
    public enum LogLevel
    {
        Verbose = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}