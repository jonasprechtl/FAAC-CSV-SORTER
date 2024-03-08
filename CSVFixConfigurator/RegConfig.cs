using Microsoft.Win32;

namespace Config{
    public class RegConfig{

        private static readonly string RegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\FAAC\\CSVFixer";

        public static string InputFile; //INPUTFILE - The file to be read
        public static string OutputFile; //OUTPUTFILE - The file to be written
        public static string ExecutionTime; //EXECTIME - The time when the task should be executed (HH:mm)

        public static string[] getValues(){
            return new string[] {InputFile, OutputFile, ExecutionTime};
        }

        public static void loadValuesFromRegistry(){
            InputFile = (string)Registry.GetValue(RegistryPath, "INPUTFILE", null) ?? "";
            OutputFile = (string)Registry.GetValue(RegistryPath, "OUTPUTFILE", null) ?? "";
            ExecutionTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null) ?? "10:00";
        }

        public static void updateValues(string InputFile, string OutputFile, string ExecutionTime){
            Registry.SetValue(RegistryPath, "INPUTFILE", InputFile);
            Registry.SetValue(RegistryPath, "OUTPUTFILE", OutputFile);
            Registry.SetValue(RegistryPath, "EXECTIME", ExecutionTime);

        	loadValuesFromRegistry();
        }
            
    }
}