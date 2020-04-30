using System;
using System.Collections.Generic;
using System.Diagnostics;
using ThinLogParser.Manage;
using ThinLogParser.Objects;

namespace ThinLogParser
{
    class Program
    {
        public static EventLog myLog;


        static void Main(string[] args)
        {
            ConfigurationReader.LoadXmlConfiguration();
            ControllerCommunication controllerCommunication = new ControllerCommunication();
            ManageOperations.LastReadInit();
            EventsRetriver eventsRetriver = new EventsRetriver();




            if (args.Length > 0)
            {
                // Extension managment behavior 
                if (args[0].ToUpper() == "CREATESCHEMA") { 
                    Console.WriteLine(controllerCommunication.createSchema());
                    Console.ReadLine();
                }

                if (args[0].ToUpper() == "HELP")
                {
                    Console.WriteLine("N/A");
                    Console.ReadLine();
                }

                if (args[0].ToUpper() == "DELETESCHEMA")
                {
                    Console.WriteLine(controllerCommunication.deleteSchema());
                    Console.ReadLine();
                }
            } else {

                // populate a list of events to push to AppDynamics events service
                eventsRetriver.populate();

                // Pushs events to events service
                controllerCommunication.publishEvent(eventsRetriver.EventsContainer);

                //Making sure that the text file is updated with latest time stamp
                ManageOperations.LastRead = DateTime.Now;
                
            }

        }

    }
}
