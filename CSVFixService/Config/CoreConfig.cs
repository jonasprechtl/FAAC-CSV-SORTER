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
        private static string OutputFile = ""; // Filepath to the output file, stored in Registry to prevent unauthorized access
        private static string execTime = ""; // "HH:MM" 24h format, stored in Registry to prevent unauthorized access
        private static string lastRun = ""; // DD-MM-YYYY HH:MM
        private static string nextRun = ""; // DD-MM-YYYY HH:MM
        private static int useAuth = 0; // 0 = do not authenticate, 1 = authenticate

        private static readonly string RegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\FAAC\\CSVFixer";

        public static string GetInputFile()
        {
            return InputFile ?? throw new Exception("InputFile not set");
        }

        public static string GetOutputFile()
        {
            return OutputFile ?? throw new Exception("OutputFile not set");
        }

        public static bool shouldUseAuth()
        {
            if (useAuth == 0)
            {
                return false;
            }
            return true;
        }

        /*
         * This logic calculates if the next run is due!
         * Before running the task, the config is read from the registry to ensure the newest Data
         */
        public static bool isNextRunDue()
        {

            //TODO: What if the time is change today to be later? It should run again, but with the current logic it will not
            readConfig();

            DateTime nextRun = DateTime.ParseExact(CoreConfig.nextRun, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            //If next run is in past
            if (nextRun < DateTime.Now)
            {
                return true;
            }

            return false;
        }

        public static void registerRun()
        {
            //Write the current time to the registry
            Registry.SetValue(RegistryPath, "LASTRUN", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

            //Write the tomorrow date and the execTime to the registry
            Registry.SetValue(RegistryPath, "NEXTRUN", DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + " " + execTime);
        }


        /* 
         * If lastrun is not set, the lastRunDateTime will be on 01-01-2000 00:00
         * If nextRun is not set, the nextRunDateTime will be on 01-01-2000 00:00 to ensure the task will run
         * Input and OutputFile will be set to "" if not defined

        */
        public static void readConfig()
        {

            verifyConfig();
            //Read the config from the registry
            //If not found, throw an exception

            OutputFile = (string)Registry.GetValue(RegistryPath, "OUTPUTFILE", null) ?? "";
            InputFile = (string)Registry.GetValue(RegistryPath, "INPUTFILE", null) ?? "";
            execTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null) ?? "";
            lastRun = (string)Registry.GetValue(RegistryPath, "LASTRUN", null) ?? "";
            nextRun = (string)Registry.GetValue(RegistryPath, "NEXTRUN", null) ?? "";
            try{
                useAuth = (int)Registry.GetValue(RegistryPath, "USEAUTH", 0);;
            } catch (System.NullReferenceException){
                useAuth = 0;
            }

            if (execTime == "")
            {
                execTime = "10:00";
            }
            if (nextRun == "")
            {
                nextRun = "01-01-2000 00:00";
            }
            if (lastRun == "")
            {
                lastRun = "01-01-2000 00:00";
            }

        }

        //if the exectime was changed, the next run should be updated. This function will be called every time the config is read to ensure correct data
        private static void verifyConfig()
        {
            string execTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null) ?? "";
            string nextRun = (string)Registry.GetValue(RegistryPath, "NEXTRUN", null) ?? "";

            if (nextRun == "")
            {
                return;
            }

            //Parse execTime and nextRun as DateTime
            DateTime execTimeDateTime = DateTime.ParseExact(execTime, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            DateTime nextRunDateTime = DateTime.ParseExact(nextRun, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);



            //check if the execTime matches the time on the nextRun
            if (execTimeDateTime.Hour != nextRunDateTime.Hour || execTimeDateTime.Minute != nextRunDateTime.Minute)
            {
                //If not, update the nextRun to the new execTime. Keep the date the same, only update the time
                Registry.SetValue(RegistryPath, "NEXTRUN", nextRunDateTime.ToString("dd-MM-yyyy") + " " + execTime);
            }


            //Update the Password and Username if they are set in the registry

            //If there is a USERNAME OR PASSWORD in the registry, set the credentials using the credentials class and delete the registry entry
            string username = (string)Registry.GetValue(RegistryPath, "USERNAME", null) ?? "";
            string password = (string)Registry.GetValue(RegistryPath, "PASSWORD", null) ?? "";

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                Credentials.setCredential(username, password);
                Registry.SetValue(RegistryPath, "USERNAME", "");
                Registry.SetValue(RegistryPath, "PASSWORD", "");
                Credentials.setCredential(username, password);
            }


        }

    }


}