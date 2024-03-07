using System;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;

namespace CSVFixService
{
    public class AuthenticatedFileAccess
    {
        public static string[] ReadAllLinesAuthenticated(string filePath, string username, string password)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                // Read the file unauthenticated
                if(!File.Exists(filePath)){
                    return new string[0];
                }
                string[] fileContents = File.ReadAllLines(filePath);
                return fileContents;
            }
            else
            {
                // Read the file authenticated

                //TODO: The server on wich to authenticate is not specified, so it does not know where to authenticate
                UserCredentials credentials = new UserCredentials(username, password);

                using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.Batch);

                try
                {
                    return WindowsIdentity.RunImpersonated(userHandle, () =>
                    {

                       if(!File.Exists(filePath)){
                           return new string[0];
                       }

                        // Execute file read operation within the impersonated context
                        string[] fileContents = File.ReadAllLines(filePath);
                        return fileContents;
                    });
                }
                catch (Exception ex)
                {
                    // Handle any exceptions related to authentication or file access
                    Console.WriteLine($"Error reading file: {ex.Message}");
                    return new string[0];
                }
            }
        }
    }
}
