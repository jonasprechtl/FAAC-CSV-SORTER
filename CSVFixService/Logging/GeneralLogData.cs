using Config;

namespace Log{
    public class GeneralLogData
    {
        public static void LogSystemInfo()
        {
            Logger.Log("---- SYSTEM INFO ----", LogLevel.Info);
            Logger.Log("Machine Name: " + Environment.MachineName, LogLevel.Info);
            Logger.Log("OS Version: " + Environment.OSVersion, LogLevel.Info);
            Logger.Log("Processor Count: " + Environment.ProcessorCount, LogLevel.Info);
            Logger.Log("System Directory: " + Environment.SystemDirectory, LogLevel.Info);
            Logger.Log("System Page Size: " + Environment.SystemPageSize, LogLevel.Info);
            Logger.Log("Is 64-bit Process: " + Environment.Is64BitProcess, LogLevel.Info);
            Logger.Log("Is 64-bit Operating System: " + Environment.Is64BitOperatingSystem, LogLevel.Info);
            Logger.Log("Current Managed Thread Id: " + Environment.CurrentManagedThreadId, LogLevel.Info);
            Logger.Log("Command Line: " + Environment.CommandLine, LogLevel.Info);
            Logger.Log("Current Working Directory: " + Environment.CurrentDirectory, LogLevel.Info);
            Logger.Log("User Name: " + Environment.UserName, LogLevel.Info);
            Logger.Log("User Domain Name: " + Environment.UserDomainName, LogLevel.Info);
            Logger.Log("CLR Version: " + Environment.Version, LogLevel.Info);
            Logger.Log("---- END SYSTEM INFO ----", LogLevel.Info);

        }

        public static void LogCurrentConfig(){
            Dictionary<string, string> allConfigSettings = CoreConfig.GetAllConfigDetails();

            Logger.Log("---- CURRENT CONFIG SETTINGS ----", LogLevel.Info);
            foreach (KeyValuePair<string, string> setting in allConfigSettings)
            {
                Logger.Log(setting.Key + ": " + setting.Value, LogLevel.Info);
            }
            Logger.Log("---- END CURRENT CONFIG SETTINGS ----", LogLevel.Info);
        }

    }
}