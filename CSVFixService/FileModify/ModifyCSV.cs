using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Config;
using FileOperations;
using Log;

namespace FileOperations
{
    public class CSVFix
    {
        //Creates a new CSV that is fixed
        public static void FixCSV()
        {

            Logger.Log("Starting to fix the CSV", LogLevel.Verbose);
            string inputFile = CoreConfig.GetInputFile();
            Logger.Log("Input CSV File: " + inputFile, LogLevel.Info);
            string outputFile = CoreConfig.GetOutputFile();
            Logger.Log("Output CSV File: " + outputFile, LogLevel.Info);

            //If the Files are not set (eg. right after initial start), the method will just return
            if(inputFile == "" || outputFile == "") {
                Logger.Log("Filenames not configured", LogLevel.Error);
                throw new InvalidOperationException("Filenames not configured");
            }

            //Those indexes start at 0
            //TODO: Maybe make those indexes configurable
            byte LicensePlateIndex = 5;
            Logger.Log("CSV License Plate Index: " + LicensePlateIndex, LogLevel.Verbose);

            byte validTillIndex = 16;
            Logger.Log("CSV Valid Till Index: " + validTillIndex, LogLevel.Verbose);

            byte counterIndex = 4;
            Logger.Log("CSV Incremental Counter Index: " + counterIndex, LogLevel.Verbose);

            char separator = ',';
            Logger.Log("CSV Separator Symbol: " + separator, LogLevel.Verbose);


            //Modify the lines
            //TODO: Maybe set the index of the License Plate and the index in the Registry to allow the user to change the indexes

            /*
            * This Logic is here to fix the CSV (CSV has a header)
            * Do not reorder the logic, as for example the deduplication could 
            * remove a not expired dataset and leave the expired one wich would cause a bug

            1. Count the amount of columns and remove all lines, that do not have the same amount of columns than the header -
            2. Remove all lines where the license place has special characters (ÄÖÜäöü are allowed) -
            3. Remove all lines where the license plate is longer than 8 characters -
            4. TODO: Maybe also remove all lines where the license plate is SHORTER than x characters -
            2. Remove all lines where the valid date is exceeded -
            6. Deduplicate the list using the license plate as unique identifier. Only keep the one with the longest valid date
            7. Add a number to each dataset (starting with 1 and increasing by 1 for each dataset)
            */

            //Read the inputFile
            string[] lines;
            if( !CoreConfig.shouldUseAuth() ){
                Logger.Log("Reading file without authentication", LogLevel.Info);
                lines = AuthenticatedAccess.readLinesAuthenticated(inputFile, null, null);
                Logger.Log("File read", LogLevel.Verbose);
            } else {
                Logger.Log("Reading file with authentication", LogLevel.Info);
                Logger.Log("Reading credentials", LogLevel.Verbose);
                (string username, string password) = Credentials.readCredential();
                Logger.Log("Credentials read", LogLevel.Verbose);
                lines = AuthenticatedAccess.readLinesAuthenticated(inputFile, username, password);
                Logger.Log("File read", LogLevel.Verbose);
            }

            //If lines is empty just return
            if(lines.Length == 0 || lines.Length == 1 ){
                Logger.Log("The Input CSV File is empty or only contains the header", LogLevel.Warning);
                return;
            }

            //Count the amount of columns in the header
            int headerColumnCount = lines[0].Split(separator).Length;
            Logger.Log("CSV Header Column Count: " + headerColumnCount, LogLevel.Info);

            string header = lines[0];

            /*
            First pass:
            Remove all lines with a different amount of columns than the header -
            Remove all lines where the license plate has special characters - 
            Remove all lines where the license plate is longer than 8 characters - 
            Remove all lines where the valid date is exceeded (format: 31/01/2016 23:59:00) -

            Start at 1, as the first line is the header
            */

            int totalLines = lines.Length - 1;
            Logger.Log("Starting first pass", LogLevel.Verbose);
            Logger.Log("Lines to process: " + totalLines, LogLevel.Verbose);

            //output of the first pass
            List<string> firstPassOutput = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {

                string[] columns = lines[i].Split(separator);

                //Check if the amount of columns is the same as the header
                if (columns.Length != headerColumnCount)
                {
                    continue;
                }

                //Check if the license plate has special characters and is max 8 characters long
                //SPACE is also a special character
                if (!Regex.IsMatch(columns[LicensePlateIndex], @"^[A-Za-z0-9ÄÖÜäöü]{0,8}$"))
                {
                    continue;
                }

                //Check if the valid date is exceeded
                if (DateTime.ParseExact(columns[validTillIndex], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) < DateTime.Now)
                {
                    continue;
                }

                firstPassOutput.Add(lines[i]);

            }
            Logger.Log("First pass done", LogLevel.Verbose);
            Logger.Log("Lines remaining after first pass: " + firstPassOutput.Count, LogLevel.Verbose);

            /*
            Second Pass: 
            Remove Duplicated license plates. only keep the one with the longest validity date
            */

            Logger.Log("Starting second pass", LogLevel.Verbose);
            Logger.Log("Lines to process: " + firstPassOutput.Count, LogLevel.Verbose);
            //Deduplicate the list using registration as the primary key
            //If duplicate is found, keep the registration with the expiration date longest in the Future.
            // Deduplication based on license plate with longest validity
            var groupedByLicensePlate = firstPassOutput
                .Select(line => new
                {
                    Line = line,
                    Columns = line.Split(separator),
                    ValidDate = DateTime.ParseExact(line.Split(separator)[validTillIndex], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                })
                .GroupBy(x => x.Columns[LicensePlateIndex])
                .Select(g => g.OrderByDescending(x => x.ValidDate).First().Line)
                .ToList();


            // Add counter to each dataset
            for (int i = 0; i < groupedByLicensePlate.Count; i++)
            {
                string[] columns = groupedByLicensePlate[i].Split(separator).ToArray();
                columns[counterIndex] = (i + 1).ToString(); // Increment counter starting from 1
                groupedByLicensePlate[i] = string.Join(separator.ToString(), columns);
            }

            Logger.Log("Second pass done", LogLevel.Verbose);
            Logger.Log("Lines remaining after second pass: " + groupedByLicensePlate.Count, LogLevel.Verbose);
            Logger.Log("Deleted " + (totalLines - groupedByLicensePlate.Count) + " incorrect lines", LogLevel.Info);
            Logger.Log("Kept " + groupedByLicensePlate.Count + " correct lines", LogLevel.Info);

            //Lastly, add the header to the groupedByLicensePlate list, to add the header to the output
            groupedByLicensePlate.Insert(0, header);

            if(CoreConfig.shouldUseAuth()){
                Logger.Log("Writing file with authentication", LogLevel.Info);
                Logger.Log("Reading credentials", LogLevel.Verbose);
                (string username, string password) = Credentials.readCredential();
                Logger.Log("Credentials read", LogLevel.Verbose);
                Logger.Log("Writing corrected CSV File", LogLevel.Verbose);
                AuthenticatedAccess.writeLinesAuthenticated(outputFile, groupedByLicensePlate.ToArray<string>(), username, password);
                Logger.Log("Corrected CSV File written successfully", LogLevel.Verbose);
            } else {
                Logger.Log("Writing file without authentication", LogLevel.Info);
                AuthenticatedAccess.writeLinesAuthenticated(outputFile, groupedByLicensePlate.Prepend(lines[0]).ToArray<string>(), null, null); // Prepend header to the output
                Logger.Log("Corrected CSV File written successfully", LogLevel.Verbose);
            }

        }
    }
}