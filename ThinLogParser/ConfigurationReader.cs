using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ThinLogParser
{
    public static class ConfigurationReader
    {

        public static string XmlPath;
        public static Dictionary<string, string> ConfigurationSet;
        public static List<string> EventsList;
        public static string TempString;
        public static char splitCharacter = ';';

        public static string PrepareXmlPath() 
        {
            return XmlPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\config.xml";
        }


        /// <summary>
        /// Parsing the xml configuration into a public static Dictionary to use it application wide
        /// </summary>
        public static void LoadXmlConfiguration()
        {
            XmlDocument xmlReader = new XmlDocument();
            xmlReader.Load(PrepareXmlPath());

            ConfigurationSet = new Dictionary<string, string>();
            EventsList = new List<string>();

            foreach (XmlNode node in xmlReader.DocumentElement.ChildNodes)
            {
                if (node.Name != "#comment") {
                    if (node.Name == "hostname" && node.InnerText == "")
                    {
                        ConfigurationSet.Add(node.Name, Environment.MachineName);
                    } else { 
                        ConfigurationSet.Add(node.Name, node.InnerText);
                    }
                }
            }

            prepareEventsList();
        }

        /// <summary>
        /// Need to check if we are going to monitor multiple event logs
        /// </summary>
        public static void prepareEventsList()
        {
            if (ConfigurationSet["windows-events-names"].Contains(";"))
            {
                EventsList = ConfigurationSet["windows-events-names"].Split(splitCharacter).ToList();   
            } 
            else
            {
                EventsList.Add(ConfigurationSet["windows-events-names"]);
            }
        }

       






    }
}
