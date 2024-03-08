using System.Diagnostics;

namespace FileOperations{
    public class AuthenticatedAccess{
        public static string[] readLinesAuthenticated(string filepath, string? username, string? password){
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)){
                return File.ReadAllLines(filepath);
            }

            //Split the filepath into file and path+
            var split = filepath.Split('\\');
            var file = split[split.Length - 1];
            //-1 to remove the \ at the end
            var path = filepath.Substring(0, filepath.Length - file.Length - 1);


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
            Process process = Process.Start(psi);

            //If timeout of 20 seconds is reached, throw an exception
            //This is to precent the registration of a new run, when it actually failed
            if (!process.WaitForExit(10_000)) {
                Console.Write(process.StandardError.ReadToEnd());
                throw new TimeoutException("Could not connect to the drive");
            }
            Console.Write(process.StandardOutput.ReadToEnd());



            //read the file from the drive
            string[] lines = File.ReadAllLines(filepath);

            //disconnect from the drive
            psi = new ProcessStartInfo("cmd", $"/C net use {path} /delete")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            process = Process.Start(psi);
            process.WaitForExit();

            return lines;
        }

        public static void writeLinesAuthenticated(string filepath, string[] lines, string? username, string? password){
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)){
                File.WriteAllLines(filepath, lines);
                return;
            }

            //Split the filepath into file and path+
            var split = filepath.Split('\\');
            var file = split[split.Length - 1];
            //-1 to remove the \ at the end
            var path = filepath.Substring(0, filepath.Length - file.Length - 1);

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

            Process process = Process.Start(psi);
            if(!process.WaitForExit(10_000)){
                Console.Write(process.StandardError.ReadToEnd());
                throw new TimeoutException("Could not connect to the drive");
            }
            Console.Write(process.StandardOutput.ReadToEnd());

            //write the file to the drive
            File.WriteAllLines(filepath, lines);

            //disconnect from the drive
            psi = new ProcessStartInfo("cmd", $"/C net use {path} /delete")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            process = Process.Start(psi);
            process.WaitForExit();

        }
    }

}