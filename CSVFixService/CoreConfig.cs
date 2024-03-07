using Microsoft.Win32;

namespace Config{

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

    private static readonly string RegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\FAAC\\CSVFixer";

    public static string GetInputFile()
    {
        return InputFile ?? throw new Exception("InputFile not set");
    }

    public static string GetOutputFile()
    {
        return OutputFile ?? throw new Exception("OutputFile not set");
    }

    /*
     * This logic calculates if the next run is due!
     * Before running the task, the config is read from the registry to ensure the newest Data
     */
    public static bool isNextRunDue()
    {

        //TODO: What if the time is change today to be later? It should run again, but with the current logic it will not
        readConfig();


        //Parse lastRun to DateTime
        DateTime lastRunDateTime = DateTime.ParseExact(lastRun, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);


        //Parse the execTime. 
        //It should be the today Date with the execTime as time
        DateTime nextRunDateTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " " + execTime, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

        if (nextRunDateTime > lastRunDateTime)
        {
            return true;
        }


        return false;
    }

    public static void registerRun()
    {
        //Write the current time to the registry
        Registry.SetValue(RegistryPath, "LASTRUN", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
    }


    /* 
     * This will throw an exception if a value would be null
     * If lastrun is not set, the lastRunDateTime will be on 01-01-2000 00:00 to ensure the next run will be executed
    */
    public static void readConfig()
    {
        //Read the config from the registry
        //If not found, throw an exception

        OutputFile = (string)Registry.GetValue(RegistryPath, "OUTPUTFILE", null) ?? "";
        InputFile = (string)Registry.GetValue(RegistryPath, "INPUTFILE", null) ?? "";
        execTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null)  ?? "";
        lastRun = (string)Registry.GetValue(RegistryPath, "LASTRUN", null)  ?? "";

        if (OutputFile == "")
        {
            throw new Exception("OUTPUTFILE not found in registry");
        }
        if (InputFile == "")
        {
            throw new Exception("INPUTFILE not found in registry");
        }
        if (execTime == "")
        {
            throw new Exception("EXECTIME not found in registry");
        }
        if (lastRun == "")
        {
            lastRun = "01-01-2000 00:00";
        }


    }
}
}