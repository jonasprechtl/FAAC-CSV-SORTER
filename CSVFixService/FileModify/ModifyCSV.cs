using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Config;
using FileOperations;

namespace FileOperations
{
    public class CSVFix
    {
        //Creates a new CSV that is fixed
        public static void FixCSV()
        {

            string inputFile = CoreConfig.GetInputFile();
            string outputFile = CoreConfig.GetOutputFile();

            //If the Files are not set (eg. right after initial start), the method will just return
            if(inputFile == "" || outputFile == "") throw new InvalidOperationException("Filenames not configured");

            //Those indexes start at 0
            byte LicensePlateIndex = 5;
            byte validTillIndex = 16;
            byte counterIndex = 4;
            char separator = ',';


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
                lines = AuthenticatedAccess.readLinesAuthenticated(inputFile, null, null);
            } else {
                (string username, string password) = Credentials.readCredential();
                lines = AuthenticatedAccess.readLinesAuthenticated(inputFile, username, password);
            }

            //If lines is empty just return
            if(lines.Length == 0 || lines.Length == 1 ){
                return;
            }

            //Count the amount of columns in the header
            int headerColumnCount = lines[0].Split(separator).Length;



            /*
            First pass:
            Remove all lines with a different amount of columns than the header -
            Remove all lines where the license plate has special characters - 
            Remove all lines where the license plate is longer than 8 characters - 
            Remove all lines where the valid date is exceeded (format: 31/01/2016 23:59:00) -

            Start at 1, as the first line is the header
            */

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

            /*
            Second Pass: 
            Remove Duplicated license plates. only keep the one with the longest validity date
            */

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

            if(CoreConfig.shouldUseAuth()){
                (string username, string password) = Credentials.readCredential();
                AuthenticatedAccess.writeLinesAuthenticated(outputFile, groupedByLicensePlate.ToArray<string>(), username, password);
            } else {
            AuthenticatedAccess.writeLinesAuthenticated(outputFile, groupedByLicensePlate.Prepend(lines[0]).ToArray<string>(), null, null); // Prepend header to the output

            }

        }
    }
}