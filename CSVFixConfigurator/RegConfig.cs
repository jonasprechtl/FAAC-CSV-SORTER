using Microsoft.Win32;

namespace Config{
    public class RegConfig{

        private static readonly string RegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\FAAC\\CSVFixer";

        public static string InputFile; //INPUTFILE - The file to be read
        public static string OutputFile; //OUTPUTFILE - The file to be written
        public static string ExecutionTime; //EXECTIME - The time when the task should be executed (HH:mm)

        public static int UseAuth = 0; // 0 = do not authenticate, 1 = authenticate

        public static string Username; //USERNAME - The username to be used for authentication (Only if UseAuth = 1)
        public static string Password; //PASSWORD - The password to be used for authentication (Only if UseAuth = 1)

        public static string[] getValues(){
            return new string[] {InputFile, OutputFile, ExecutionTime, UseAuth.ToString()};
        }

        public static void loadValuesFromRegistry(){
            InputFile = (string)Registry.GetValue(RegistryPath, "INPUTFILE", null) ?? "";
            OutputFile = (string)Registry.GetValue(RegistryPath, "OUTPUTFILE", null) ?? "";
            ExecutionTime = (string)Registry.GetValue(RegistryPath, "EXECTIME", null) ?? "10:00";
            UseAuth = (int)Registry.GetValue(RegistryPath, "USEAUTH", 0);
        }

        public static void updateValues(string InputFile, string OutputFile, string ExecutionTime, int UseAuth){
            Registry.SetValue(RegistryPath, "INPUTFILE", InputFile);
            Registry.SetValue(RegistryPath, "OUTPUTFILE", OutputFile);
            Registry.SetValue(RegistryPath, "EXECTIME", ExecutionTime);
            Registry.SetValue(RegistryPath, "USEAUTH", UseAuth);

        	loadValuesFromRegistry();
        }

        public static void updateValues(string InputFile, string OutputFile, string ExecutionTime, int UseAuth, string Username, string Password){
            if(UseAuth == 1){
                //In case someone calls this function but UseAuth is 0, just ignore the username and password
                Registry.SetValue(RegistryPath, "USERNAME", Username);
                Registry.SetValue(RegistryPath, "PASSWORD", Password);
            }
            updateValues(InputFile, OutputFile, ExecutionTime, UseAuth);
        }
            
    }
}