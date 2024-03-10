using CredentialManagement;
using Log;

namespace Config{
    public class Credentials{

        static readonly string credTarget = "FAAC CSV Fixer Network File Access";

        public static void setCredential(string username, string password){
            Logger.Log("Setting Credentials", LogLevel.Info);
            Logger.Log("Credential Username: " + username, LogLevel.Verbose);
            var credential = new Credential{
                Target = credTarget,
                Username = username,
                Password = password,
                PersistanceType = PersistanceType.Enterprise
            };

            Logger.Log("Saving Credentials", LogLevel.Verbose);
            bool success =  credential.Save();
            if(success){
                Logger.Log("Credentials saved successfully", LogLevel.Info);
            } else {
                Logger.Log("Failed to save the credential. Please ensure the details are correct and the environment supports saving credentials.", LogLevel.Error);
                throw new InvalidOperationException("Failed to store the credential. Please ensure the details are correct and the environment supports saving credentials.");
            }
        }


        /*
            Returns ("","") if no credentials are set. The Program should try it with its own credentials if no credentials are set.
        */
        public static (string Username, string Password) readCredential(){
            Logger.Log("Reading credentials", LogLevel.Info);

            var credential = new Credential { Target = credTarget };
            bool success = credential.Load();

            if (!success)
            {
               Logger.Log("Failed to load the credential. Returning empty username and password.", LogLevel.Warning);
                return ("","");
            }

            Logger.Log($"Credential loaded for user: {credential.Username}", LogLevel.Verbose);

            return (credential.Username, credential.Password);
        }

        public static void deleteCredentials()
        {
            Logger.Log("Deleting credentials", LogLevel.Info);

            var credential = new Credential { Target = credTarget };
            bool success = credential.Delete();

            if (!success)
            {
                Logger.Log("Failed to delete the credential. Please ensure the credential exists and the environment supports deleting credentials.", LogLevel.Error);
                throw new InvalidOperationException("Failed to delete the credential. Please ensure the credential exists and the environment supports deleting credentials.");
            }

            Logger.Log("Credential deleted successfully.", LogLevel.Info);
        }

    }
}