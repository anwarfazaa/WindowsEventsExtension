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
        EventLog eventLog;

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
                    eventLog = new EventLog(Event);
                    eventsSize = eventLog.Entries.Count - 1;

                    for (int i = eventsSize; i >= 0; i--)
                    {
                        // comparing entry against time stamp
                        if (ManageOperations.LastRead <= eventLog.Entries[i].TimeWritten)
                        {
                            break;
                        }
                        eventContainer.Add(new Event(Event, eventLog.Entries[i].EntryType.ToString(), eventLog.Entries[i].TimeWritten, ConfigurationReader.ConfigurationSet["hostname"], eventLog.Entries[i].Source, eventLog.Entries[i].Message, eventLog.Entries[i].InstanceId));
                    }
                    eventLog.Close();

                }
                else
                {
                    // Second condition check if both log and a specific log match
                    splitContainer = Event.Split(seprator, StringSplitOptions.RemoveEmptyEntries);
                    SingleEventEntry = splitContainer[0];
                    EventSource = splitContainer[1];
                    eventLog = new EventLog(SingleEventEntry);
                    eventsSize = eventLog.Entries.Count - 1;
                    if (splitContainer.Length == 2)
                    {
                        for (int i = eventsSize; i >= 0; i--)
                        {
                            if (ManageOperations.LastRead <= eventLog.Entries[i].TimeWritten)
                            {
                                break;
                            }
                            if (eventLog.Entries[i].Source.ToString() == EventSource)
                            {
                                eventContainer.Add(new Event(SingleEventEntry, eventLog.Entries[i].EntryType.ToString(), eventLog.Entries[i].TimeWritten, ConfigurationReader.ConfigurationSet["hostname"], eventLog.Entries[i].Source, eventLog.Entries[i].Message, eventLog.Entries[i].InstanceId));
                            }
                        }
                    }
                    else if (splitContainer.Length == 3)
                    {
                        EventLevel = splitContainer[2];
                        for (int i = eventsSize; i >= 0; i--)
                        {
                            if (ManageOperations.LastRead <= eventLog.Entries[i].TimeWritten)
                            {
                                break;
                            }
                            if (eventLog.Entries[i].Source.ToString() == EventSource && eventLog.Entries[i].EntryType.ToString() == EventLevel)
                            {
                                eventContainer.Add(new Event(SingleEventEntry, eventLog.Entries[i].EntryType.ToString(), eventLog.Entries[i].TimeWritten, ConfigurationReader.ConfigurationSet["hostname"], eventLog.Entries[i].Source, eventLog.Entries[i].Message, eventLog.Entries[i].InstanceId));
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
