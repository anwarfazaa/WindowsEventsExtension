using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ThinLogParser.Manage
{
    public static class ManageOperations
    {

        public static string fileContent;
        public static DateTime dateContent;
        public static string currentTime;

        /// <summary>
        /// For validating last time events were uploaded to events service
        /// </summary>
        public static DateTime LastRead
        {

            get
            {
                return DateTime.Parse(currentTime);
            }
            set
            {
                File.WriteAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt", value.ToString("o"));
            }

        }


        //public static DateTime LastRead
        //{

        //    get
        //    {
        //        fileContent = File.ReadAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt");
        //        // Check if file empty at first
        //        if (fileContent == "")
        //        {
        //            currentTime = DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
        //            File.WriteAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt", currentTime);
        //            dateContent = DateTime.Parse(currentTime); ;
        //        }
        //        else
        //        {
        //            dateContent = DateTime.Parse(fileContent);
        //        }
        //        return dateContent;
        //    }
        //    set
        //    {
        //        File.WriteAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt", value.ToString("o"));
        //    }

        //}


        public static void LastReadInit()
        {
            if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt"))
            {
                fileContent = File.ReadAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt");
                // if file is empty populate file with current timestamp
                if (fileContent == "")
                {
                    currentTime = DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                    File.WriteAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt", currentTime);
                    dateContent = DateTime.Parse(currentTime); 
                } else
                {
                    dateContent = DateTime.Parse(fileContent);
                }
            } else
            {
                // create new file with current timestamp.
                currentTime = DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                File.WriteAllText(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\lastdate.txt", currentTime);
            }
        }



    }
}
