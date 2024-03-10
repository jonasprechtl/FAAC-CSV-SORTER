using CredentialManagement;

namespace Config{
    public class Credentials{

        static readonly string credTarget = "FAAC CSV Fixer Network File Access";

        public static void setCredential(string username, string password){
            var credential = new Credential{
                Target = credTarget,
                Username = username,
                Password = password,
                PersistanceType = PersistanceType.Enterprise
            };

            bool success =  credential.Save();

            if(!success) throw new InvalidOperationException("Failed to store the credential. Please ensure the details are correct and the environment supports saving credentials.");
        }


        /*
            Returns ("","") if no credentials are set. The Program should try it with its own credentials if no credentials are set.
        */
        public static (string Username, string Password) readCredential(){
            var credential = new Credential { Target = credTarget };

            bool success = credential.Load();

            if (!success)
            {
                return ("","");
            }

            return (credential.Username, credential.Password);
        }


        public static void deleteCredentials()
         {
            var credential = new Credential { Target = credTarget };

            bool success = credential.Delete();

            if (!success)
            {
                throw new InvalidOperationException("Failed to delete the credential. Please ensure the credential exists and the environment supports deleting credentials.");
            }
        }

    }
}