using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinLogParser.Objects
{
    class Event
    {
        public string LoggedTime;
        public string LogName;
        public string EventLevel;
        public string HostName;
        public string Source;
        public string Message;
        public long EventID;

        public Event(string logName, string eventLevel, DateTime loggedTime, string hostname, string source, string message, long eventID)
        {
            LoggedTime = loggedTimeToAppdFormat(loggedTime);
            LogName = logName;
            EventLevel = eventLevel;
            HostName = hostname;
            Source = source;
            Message = message;
            EventID = eventID;
        }

        // Formatting it to ISO 8601 to be supported by AppDynamics events service.
        public static string loggedTimeToAppdFormat(DateTime loggedTime)
        {
            return loggedTime.ToString("s",System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
