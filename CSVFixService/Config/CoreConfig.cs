using System.Dynamic;
using Log;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Win32;

namespace Config
{

    public static class CoreConfig
    {
        /*
            Store a Time when to run the task

            In the registry, save a lastrun value with time of current run

            Make a function isRunDue() wich check if the next run should happen now or in the past.
            The function checks the Run today has already happened and returns true or false.
        */



        private static string InputFile = ""; // Filepath to the input file, stored in Registry to prevent unauthorized access
        private static string InputFile2 = ""; // Filepath to the second input file, stored in Registry to prevent unauthorized access
        private static string OutputFile = ""; // Filepath to the output file, stored in Registry to prevent unauthorized access
        private static string execTime = ""; // "HH:MM" 24h format, stored in Registry to prevent unauthorized access
        private static string lastRun = ""; // DD-MM-YYYY HH:MM
        private static string nextRun = ""; // DD-MM-YYYY HH:MM
        private static int useAuth = 0; // 0 = do not authenticate, 1 = authenticate
        private static int manualRun = 0; // 0 = do not run, 1 = run on next loop

        private static bool initialLaunchDetected = false;

        private static readonly string RegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\FAAC\\CSVFixer";

        public static string GetInputFile()
        {

            Logger.Log("Reading InputFile from CoreConfig", LogLevel.Verbose);

            if (InputFile == null || InputFile == "")
            {
                Logger.Log("InputFile not set", LogLevel.Error);
                throw new System.Exception("InputFile not set");
            }

            Logger.Log("InputFile: " + InputFile, LogLevel.Verbose);
            return InputFile;
        }

        public static string GetInputFile2()
        {

            Logger.Log("Reading InputFile2 from CoreConfig", LogLevel.Verbose);

            if(InputFile2 == null || InputFile2 == "")
            {
                Logger.Log("InputFile2 not set", LogLevel.Error);
                throw new System.Exception("InputFile2 not set");
            }

            Logger.Log("InputFile2: " + InputFile2, LogLevel.Verbose);
            return InputFile2;
        }

        public static string GetOutputFile()
        {

            Logger.Log("Reading OutputFile from CoreConfig", LogLevel.Verbose);

            if (OutputFile == null || OutputFile == "")
            {
                Logger.Log("OutputFile not set", LogLevel.Error);
                throw new System.Exception("OutputFile not set");
            }
            Logger.Log("OutputFile: " + OutputFile, LogLevel.Verbose);
            return OutputFile;
        }

        public static bool shouldUseAuth()
        {
            Logger.Log("Reading UseAuth from Registry", LogLevel.Verbose);
            if (useAuth == 0)
            {
                Logger.Log("Not using Auth because UseAuth: false", LogLevel.Verbose);
                return false;
            }
            Logger.Log("Using Auth because UseAuth: true", LogLevel.Verbose);
            return true;
        }

        /*
         * This logic calculates if the next run is due!
         * Before running the task, the config is read from the registry to ensure the newest Data
         */
        public static bool isNextRunDue()
        {

            Logger.Log("Checking if the next run is due", LogLevel.Verbose);

            Logger.Log("Refreshing Config", LogLevel.Verbose);
            readConfig();
            Logger.Log("Config Refreshed", LogLevel.Verbose);

            Logger.Log("Checking if the next run is in the past or manual run was requested", LogLevel.Verbose);
            DateTime nextRun = DateTime.ParseExact(CoreConfig.nextRun, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            //If next run is in past
            if (nextRun < DateTime.Now)
            {
                Logger.Log("Next run is in the past", LogLevel.Verbose);
                return true;
            }

            if (manualRun == 1)
            {
                Logger.Log("Manual Run is set, running the task", LogLevel.Verbose);
                return true;
            }

            Logger.Log("Next run is not in the past", LogLevel.Verbose);
            return false;
        }

        public static void registerRun()
        {
            Logger.Log("Registering the run", LogLevel.Verbose);
            //Write the current time to the registry
            Registry.SetValue(RegistryPath, "LASTRUN", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));


            /*
            Consider this case:
            It is 10:00 01.01.2024
            Next run is 11:00 01.01.2024

            Now if there is a Manual Run Today, it will also set the Time of the next run to 11:00 02.01.2024 even if the scheduled run did not yet execute
            
            So there is a check, if the next execution of the run is today or tomorrow:
            Scheduled Run: 11:00
            If it is 10:00 01.01.2024 - set next run to 11:00 01.01.2024
            If it is 12:00 01.01.2024 - set next run to 11:00 02.01.20024
            */

            DateTime nextRun = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " " + execTime, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            //if nextRun lies in the past, just add a day as the execTime is already in the past
            if(nextRun < DateTime.Now){
                Logger.Log($"The next scheduled Run is Tomorrow, as today the execTime {execTime} is already in the past for today", LogLevel.Verbose);
                nextRun = nextRun.AddDays(1);
            } else {
                Logger.Log($"The next schedules Run is Today, as the execTime {execTime} is in the future for today", LogLevel.Verbose);
            }


            //Write the tomorrow date and the execTime to the registry
            Registry.SetValue(RegistryPath, "NEXTRUN", nextRun.ToString("dd-MM-yyyy HH:mm"));

            //Set manualRun to 0
            Registry.SetValue(RegistryPath, "MANUALRUN", 0);

            Logger.Log("Run registered successfully", LogLevel.Verbose);
        }


        /* 
         * If lastrun is not set, the lastRunDateTime will be on 01-01-2000 00:00
         * If nextRun is not set, the nextRunDateTime will be on 01-01-2000 00:00 to ensure the task will run
         * Input and OutputFile will be set to "" if not defined

        */
        public static void readConfig()
        {
            Logger.Log("Reading Config from Registry", LogLevel.Verbose);

            Logger.Log("Verifying and correcting the config", LogLevel.Verbose);
            verifyConfig();
            Logger.Log("Config verified and corrected", LogLevel.Verbose);
            //Read the config from the registry
            //If not found, throw an exception

            Logger.Log("Reading Data from Registry", LogLevel.Verbose);
            OutputFile = (string)Registry.GetValue(RegistryPath, "OUTPUTFILE", null) ?? "";
            InputFile = (string)Registry.GetValue(RegistryPath, "INPUTFILE", null) ?? "";
            InputFile2 = (string)Registry.GetValue(RegistryPath, "INPUTFILE2", null) ?? "";
            execTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null) ?? "";
            lastRun = (string)Registry.GetValue(RegistryPath, "LASTRUN", null) ?? "";
            nextRun = (string)Registry.GetValue(RegistryPath, "NEXTRUN", null) ?? "";

            try
            {
                useAuth = (int)Registry.GetValue(RegistryPath, "USEAUTH", 0); ;
            }
            catch (System.NullReferenceException)
            {
                Logger.Log("UserAuth not found in Registry, defaulting to false", LogLevel.Warning);
                useAuth = 0;
            }

            try
            {
                manualRun = (int)Registry.GetValue(RegistryPath, "MANUALRUN", 0);
            }
            catch (System.NullReferenceException)
            {
                Logger.Log("ManualRun not found in Registry, defaulting to false", LogLevel.Warning);
                manualRun = 0;
            }

            if (execTime == "")
            {
                Logger.Log("ExecTime not set, defaulting to 10:00", LogLevel.Warning);
                execTime = "10:00";
                Logger.Log("This seems to be the first launch of the Software, setting initialLaunchDetected to true", LogLevel.Verbose);
                initialLaunchDetected = true;
            }
            else
            {
                Logger.Log("ExecTime is already set, which means this is not the first launch of the Software", LogLevel.Verbose);
                initialLaunchDetected = false;
            }

            if (nextRun == "")
            {
                Logger.Log("NextRun not set, defaulting to 01-01-2000 00:00", LogLevel.Warning);
                nextRun = "01-01-2000 00:00";
            }
            if (lastRun == "")
            {
                Logger.Log("LastRun not set, defaulting to 01-01-2000 00:00", LogLevel.Warning);
                lastRun = "01-01-2000 00:00";
            }

            Logger.Log("Data read from Registry", LogLevel.Verbose);
            Logger.Log("OutputFile: " + OutputFile, LogLevel.Verbose);
            Logger.Log("InputFile: " + InputFile, LogLevel.Verbose);
            Logger.Log("InputFile2: " + InputFile2, LogLevel.Verbose);
            Logger.Log("ExecTime: " + execTime, LogLevel.Verbose);
            Logger.Log("LastRun: " + lastRun, LogLevel.Verbose);
            Logger.Log("NextRun: " + nextRun, LogLevel.Verbose);
            Logger.Log("UseAuth: " + useAuth, LogLevel.Verbose);
            Logger.Log("ManualRun: " + manualRun, LogLevel.Verbose);
        }

        //if the exectime was changed, the next run should be updated. This function will be called every time the config is read to ensure correct data
        private static void verifyConfig()
        {
            Logger.Log("Verifying Config", LogLevel.Verbose);




            //If there is a USERNAME OR PASSWORD in the registry, set the credentials using the credentials class and delete the registry entry
            //Has to be before the nextRun check, as the function will return if Nextrun does not yet exist, keeping the credentials in the registry 
            // longer than necessary

            Logger.Log("Checking for credentials in Registry", LogLevel.Verbose);
            string username = (string)Registry.GetValue(RegistryPath, "USERNAME", null) ?? "";
            string password = (string)Registry.GetValue(RegistryPath, "PASSWORD", null) ?? "";

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                Logger.Log("Credentials found in Registry", LogLevel.Verbose);
                Credentials.setCredential(username, password);
                Registry.SetValue(RegistryPath, "USERNAME", "");
                Registry.SetValue(RegistryPath, "PASSWORD", "");
                Logger.Log("Credentials set in Credentialstore", LogLevel.Verbose);
            }


            Logger.Log("Reading Nextrun and Exectime from Registry", LogLevel.Verbose);
            string execTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null) ?? "";
            string nextRun = (string)Registry.GetValue(RegistryPath, "NEXTRUN", null) ?? "";
            Logger.Log("Data read from Registry", LogLevel.Verbose);


            //If nextRun is not set, do nothing. 
            //If it is not set, the task will run immediately as there are default values in the readConfig function
            //nextRun is then set by the registerRun function
            if (nextRun == "")
            {
                Logger.Log("NextRun is not set, skipping verification. This is most likely a first time run of this Software.", LogLevel.Verbose);
                return;
            }


            Logger.Log("Parsing nextRun and execTime", LogLevel.Verbose);
            //Parse execTime and nextRun as DateTime
            DateTime todaysExecDateTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " " + execTime, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            DateTime nextRunDateTime = DateTime.ParseExact(nextRun, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            Logger.Log("nextRun and execTime parsed", LogLevel.Verbose);


            //check if the execTime matches the time on the nextRun
            if (todaysExecDateTime.Hour != nextRunDateTime.Hour || todaysExecDateTime.Minute != nextRunDateTime.Minute)
            {
                Logger.Log("ExecTime and nextRun do not match, most likely the config was changed very recently", LogLevel.Verbose);

                

                //if execTime is in the past for today, change the nextRunDate to tomorrow
                //else (execTime is not in past for today), change the nextRun Date to today
                //Note that nextRun might already be tomorrow, maybe it needs to be set back to today
                DateTime currentDateTime = DateTime.Now;

                //The todays exectime is also used for next day, this is just so the name is not as confusing
                DateTime nextRunExecDateTime = todaysExecDateTime;
                

                if(nextRunExecDateTime < currentDateTime){
                    Logger.Log("ExecTime is in the past for today, nextRun is tomorrow", LogLevel.Verbose);
                    nextRunExecDateTime = nextRunExecDateTime.AddDays(1);
                } else {
                    Logger.Log("ExecTime is not past for today, nextRun to today", LogLevel.Verbose);
                }



                Registry.SetValue(RegistryPath, "NEXTRUN", nextRunExecDateTime.ToString("dd-MM-yyyy HH:mm"));
                Logger.Log("NextRun updated to match the new ExecTime", LogLevel.Verbose);
            }

        }

        public static Dictionary<string, string> GetAllConfigDetails()
        {
            Logger.Log("Getting all Config Details", LogLevel.Verbose);
            readConfig();
            Dictionary<string, string> configDetails = new Dictionary<string, string>
            {
                { "InputFile", InputFile },
                { "InputFile2", InputFile2 },
                { "OutputFile", OutputFile },
                { "ExecTime", execTime },
                { "LastRun", lastRun },
                { "NextRun", nextRun },
                { "UseAuth", useAuth == 1 ? "true" : "false"},
                { "InitialLaunchDetected", initialLaunchDetected ? "true" : "false"},
                { "ManualRun on next iteration", manualRun == 1 ? "true" : "false"}
            };
            Logger.Log("Returning Config Details", LogLevel.Verbose);
            return configDetails;
        }

    }



}