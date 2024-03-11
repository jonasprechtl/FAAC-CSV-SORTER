using System.Diagnostics;
using Log;


/*
Important note: 
If the imput is eg. \\51.12.48.50\drive\kfz.csv and output is eg. C:\temp\kfz.new.csv
the net use command will not work but will NOT throw an error.

The Write (to output) will continue as normal without authentication on the local drive.

So if one needs authentication and the other file is local, just input the credentials and the function will work as normal.
*/
namespace FileOperations
{
    public class AuthenticatedAccess
    {
        public static string[] readLinesAuthenticated(string filepath, bool deleteAfterRead, string? username, string? password)
        {

            Logger.Log("Reading File" + filepath, LogLevel.Verbose);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Logger.Log("No Username and / or Password provided", LogLevel.Verbose);
                Logger.Log("Reading File without Authentication", LogLevel.Verbose);

                string[] readlines = File.ReadAllLines(filepath);

                if (deleteAfterRead)
                {
                    Logger.Log("Deleting Source CSV File", LogLevel.Verbose);
                    File.Delete(filepath);
                    Logger.Log("Successfully deleted CSV File", LogLevel.Verbose);
                }
                return readlines;
            }

            Logger.Log("Reading File with Authentication", LogLevel.Verbose);
            Logger.Log("Recreating Filepath and Filename from complete path", LogLevel.Verbose);

            //Split the filepath into file and path+
            var split = filepath.Split('\\');
            var file = split[split.Length - 1];
            //-1 to remove the \ at the end
            var path = filepath.Substring(0, filepath.Length - file.Length - 1);

            Logger.Log("Recreated Filepath: " + path, LogLevel.Verbose);
            Logger.Log("Recreated Filename: " + file, LogLevel.Verbose);


            Logger.Log("Connecting to Drive for File Access", LogLevel.Verbose);
            //connect to drive using "net use \\51.12.48.50\drive /user:prechtl <pwd>"
            string command = $@"/C net use {path} /user:{username} {password}";
            var psi = new ProcessStartInfo("cmd")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = command
            };
            Logger.Log("Starting Process to connect to Drive", LogLevel.Verbose);
            Process process = Process.Start(psi);

            //If timeout of 20 seconds is reached, throw an exception
            //This is to precent the registration of a new run, when it actually failed
            if (!process.WaitForExit(10_000))
            {
                Logger.Log("Could not connect to the drive because of Timeout", LogLevel.Error);
                Logger.Log("Error Output of Subprocess:" + process.StandardError.ReadToEnd(), LogLevel.Error);
                throw new TimeoutException("Could not connect to the drive");
            }

            Logger.Log("Connected to Drive", LogLevel.Verbose);
            Logger.Log("Output of Subprocess:" + process.StandardOutput.ReadToEnd(), LogLevel.Verbose);

            Logger.Log("Reading Data from CSV File", LogLevel.Verbose);
            string[] lines = File.ReadAllLines(filepath);
            Logger.Log("Data read from CSV File", LogLevel.Verbose);

            if (deleteAfterRead)
            {
                Logger.Log("Deleting Source CSV File", LogLevel.Verbose);
                File.Delete(filepath);
                Logger.Log("Successfully deleted CSV File", LogLevel.Verbose);
            }

            Logger.Log("Disconnecting from Drive", LogLevel.Verbose);
            //disconnect from the drive
            psi = new ProcessStartInfo("cmd", $"/C net use {path} /delete")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            Logger.Log("Starting Process to disconnect from Drive", LogLevel.Verbose);
            process = Process.Start(psi);

            /*
            * If the timeout of 10 Seconds is not reached, just continue
            * Throwing an error would not help, as the file is already read
            * This would only make the process more error prone
            * This is only a warning, as the process will continue as normal
            */
            bool processexitsuccess = process.WaitForExit(10_000);
            if (processexitsuccess)
            {
                Logger.Log("Disconnected from Drive", LogLevel.Verbose);
            }
            else
            {
                Logger.Log("Could not disconnect from the drive because of Timeout. The Execution will continue as normal.", LogLevel.Warning);
                Logger.Log("Error Output of Subprocess:" + process.StandardError.ReadToEnd(), LogLevel.Warning);
            }

            return lines;
        }

        public static void writeLinesAuthenticated(string filepath, string[] lines, string? username, string? password)
        {

            Logger.Log("Writing File" + filepath, LogLevel.Verbose);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Logger.Log("No Username and / or Password provided", LogLevel.Verbose);
                Logger.Log("Writing File without Authentication", LogLevel.Verbose);

                File.WriteAllLines(filepath, lines);
                return;
            }

            Logger.Log("Recreating Filepath and Filename from complete path", LogLevel.Verbose);
            //Split the filepath into file and path+
            var split = filepath.Split('\\');
            var file = split[split.Length - 1];
            //-1 to remove the \ at the end
            var path = filepath.Substring(0, filepath.Length - file.Length - 1);
            Logger.Log("Recreated Filepath: " + path, LogLevel.Verbose);
            Logger.Log("Recreated Filename: " + file, LogLevel.Verbose);

            Logger.Log("Connecting to Drive for File Access", LogLevel.Verbose);
            // connect to drive 
            string command = $@"/C net use {path} /user:{username} {password}";
            var psi = new ProcessStartInfo("cmd")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = command
            };
            Logger.Log("Starting Process to connect to Drive", LogLevel.Verbose);
            Process process = Process.Start(psi);
            if (!process.WaitForExit(10_000))
            {
                Logger.Log("Could not connect to the drive because of Timeout", LogLevel.Error);
                Logger.Log("Error Output of Subprocess:" + process.StandardError.ReadToEnd(), LogLevel.Error);
                throw new TimeoutException("Could not connect to the drive");
            }
            Logger.Log("Connected to Drive", LogLevel.Verbose);
            Logger.Log("Output of Subprocess:" + process.StandardOutput.ReadToEnd(), LogLevel.Verbose);

            Logger.Log("Writing Data to CSV File", LogLevel.Verbose);
            File.WriteAllLines(filepath, lines);
            Logger.Log("Data written to CSV File", LogLevel.Verbose);

            //disconnect from the drive
            Logger.Log("Disconnecting from Drive", LogLevel.Verbose);
            psi = new ProcessStartInfo("cmd", $"/C net use {path} /delete")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            Logger.Log("Starting Process to disconnect from Drive", LogLevel.Verbose);
            process = Process.Start(psi);

            bool processexitsuccess = process.WaitForExit(10_000);
            if (processexitsuccess)
            {
                Logger.Log("Disconnected from Drive", LogLevel.Verbose);
            }
            else
            {
                Logger.Log("Could not disconnect from the drive because of Timeout. The Execution will continue as normal.", LogLevel.Warning);
                Logger.Log("Error Output of Subprocess:" + process.StandardError.ReadToEnd(), LogLevel.Warning);
            }
        }
    }

}