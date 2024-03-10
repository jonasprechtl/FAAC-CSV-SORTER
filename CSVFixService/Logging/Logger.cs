using System.Reflection;

namespace Log {
    public class Logger {

        private static string logFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ServiceLog.log");

        public static void Log(string message, LogLevel level) {
            // Log the message to a file and the Console.
            string purifiedMessage = purifyLogMessage(message, level);
            Console.WriteLine(purifiedMessage);
            File.AppendAllText(logFile, purifiedMessage + Environment.NewLine);
        }

        private static string purifyLogMessage(string message, LogLevel level) {
            //Add the timestamp to the message
            message = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] <{level}> - {message}";
            return message;
        }
    }
    public enum LogLevel {
        Verbose,
        Info,
        Warning,
        Error
    }
}