using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinLogParser.Objects;

namespace ThinLogParser.Manage
{
    class EventsRetriver
    {
        List<Event> eventContainer;
        EventLog myLog;

        public EventsRetriver() {

            // Aquire configuration values
            ConfigurationReader.LoadXmlConfiguration();
            eventContainer = new List<Event>();

        }


        public void populate()
        {
            string SingleEventEntry;
            string EventSource;
            string EventLevel;
            string[] splitContainer;
            int eventsSize;

            //Preparing seprators Values
            String[] seprator = { "->" };
            // Prepare events container
            // first condition, if whole event log is required
            foreach (string Event in ConfigurationReader.EventsList)
            {

                if (!Event.Contains("->"))
                {
                    myLog = new EventLog(Event);
                    eventsSize = myLog.Entries.Count - 1;

                    for (int i = eventsSize; i > 0; i--)
                    {
                        // comparing entry against time stamp
                        if (ManageOperations.LastRead >= myLog.Entries[i].TimeWritten)
                        {
                            break;
                        }
                        eventContainer.Add(new Event(Event, myLog.Entries[i].EntryType.ToString(), myLog.Entries[i].TimeWritten, ConfigurationReader.ConfigurationSet["hostname"], myLog.Entries[i].Source, myLog.Entries[i].Message, myLog.Entries[i].InstanceId));
                    }
                    myLog.Close();

                }
                else
                {
                    // Second condition check if both log and a specific log match
                    splitContainer = Event.Split(seprator, StringSplitOptions.RemoveEmptyEntries);
                    SingleEventEntry = splitContainer[0];
                    EventSource = splitContainer[1];
                    myLog = new EventLog(SingleEventEntry);
                    eventsSize = myLog.Entries.Count - 1;
                    if (splitContainer.Length == 2)
                    {
                        for (int i = eventsSize; i > 0; i--)
                        {
                            if (ManageOperations.LastRead >= myLog.Entries[i].TimeWritten)
                            {
                                break;
                            }
                            if (myLog.Entries[i].Source.ToString() == EventSource)
                            {
                                eventContainer.Add(new Event(SingleEventEntry, myLog.Entries[i].EntryType.ToString(), myLog.Entries[i].TimeWritten, ConfigurationReader.ConfigurationSet["hostname"], myLog.Entries[i].Source, myLog.Entries[i].Message, myLog.Entries[i].InstanceId));
                            }
                        }
                    }
                    else if (splitContainer.Length == 3)
                    {
                        EventLevel = splitContainer[2];
                        for (int i = eventsSize; i > 0; i--)
                        {
                            if (ManageOperations.LastRead >= myLog.Entries[i].TimeWritten)
                            {
                                break;
                            }
                            if (myLog.Entries[i].Source.ToString() == EventSource && myLog.Entries[i].EntryType.ToString() == EventLevel)
                            {
                                eventContainer.Add(new Event(SingleEventEntry, myLog.Entries[i].EntryType.ToString(), myLog.Entries[i].TimeWritten, ConfigurationReader.ConfigurationSet["hostname"], myLog.Entries[i].Source, myLog.Entries[i].Message, myLog.Entries[i].InstanceId));
                            }
                        }
                    }
                }
            }
        }

        public List<Event> EventsContainer
        {
            get
            {
                return eventContainer;
            }
        }


    }
}
