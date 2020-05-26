using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThinLogParser.Objects;

namespace ThinLogParser
{
    /// <summary>
    /// Maybe convert it into static class
    /// </summary>
    class ControllerCommunication
    {
        public RestClient client;
        public Dictionary<string, string> ConfigurationSet;
        public RestRequest RestRequestBody;
        private JObject jsonRequest;

        public ControllerCommunication()
        {
            ConfigurationReader.LoadXmlConfiguration();
            ConfigurationSet = ConfigurationReader.ConfigurationSet;
        }

        public string createSchema()
        {
            client = new RestClient(ConfigurationSet["event-service-endpoint"] + "/events/schema/" + ConfigurationSet["events-schema-name"]);
            JObject jsonRequest = new JObject(
            new JProperty("schema",
                new JObject(
                    new JProperty("LoggedTime", "date"),
                    new JProperty("LogName", "string"),
                    new JProperty("EventLevel", "string"),
                    new JProperty("Hostname", "string"),
                    new JProperty("Source" , "string"),
                    new JProperty("Message", "string"),
                    new JProperty("EventID", "float"))));
            RestRequestBody = new RestRequest(Method.POST);
            //Preparing headers
            RestRequestBody.AddHeader("X-Events-API-AccountName", ConfigurationSet["global-account"]);
            RestRequestBody.AddHeader("X-Events-API-Key", ConfigurationSet["api-key"]);
            RestRequestBody.AddHeader("Content-type", "application/vnd.appd.events+json;v=2");
            RestRequestBody.AddHeader("Accept", "application/vnd.appd.events+json;v=2");
            //Preparing content
            RestRequestBody.AddParameter("application/vnd.appd.events+json;v=2", jsonRequest.ToString(), ParameterType.RequestBody);
            var action = client.Execute(RestRequestBody);
            return action.StatusDescription;
        }

        /// <summary>
        /// Deleting Schema , please be cautios when using this feature, its purpose is for schemas creation rollback.
        /// Providing wrong Schema may cause loss of data and corrurping analytics service
        /// </summary>
        /// <returns>Events service response</returns>
        public string deleteSchema()
        {
            client = new RestClient(ConfigurationSet["event-service-endpoint"] + "/events/schema/" + ConfigurationSet["events-schema-name"]);
            RestRequestBody = new RestRequest(Method.DELETE);
            RestRequestBody.AddHeader("X-Events-API-AccountName", ConfigurationSet["global-account"]);
            RestRequestBody.AddHeader("X-Events-API-Key", ConfigurationSet["api-key"]);
            RestRequestBody.AddHeader("Accept", "application/vnd.appd.events+json;v=2");
            var action = client.Execute(RestRequestBody);
            return action.StatusDescription;
        }

        /// <summary>
        /// To do , create batches of request (each request to contain 20~ entries, also check if size is less or equal to 20 kb as per documentation)
        /// </summary>
        /// <param name="events">The events list provided in the configuration</param>
        public void publishEvent(List<Event> events)
        {
            
            //prepare batch before sending (5 entries)
            for (int i=0; i < events.Count; i++) { 
                
                jsonRequest = new JObject(new JProperty("LoggedTime", events[i].LoggedTime), new JProperty("LogName", events[i].LogName), new JProperty("EventLevel", events[i].EventLevel), new JProperty("Hostname", events[i].HostName), new JProperty("Source", events[i].Source), new JProperty("Message", events[i].Message), new JProperty("EventID", events[i].EventID));
                pushBatch(jsonRequest);



            }
        }

        public void pushBatch(JObject jsonRequest)
        {
            RestRequestBody = new RestRequest(Method.POST);
            client = new RestClient(ConfigurationSet["event-service-endpoint"] + "/events/publish/" + ConfigurationSet["events-schema-name"]);
            //Preparing headers
            RestRequestBody.AddHeader("X-Events-API-AccountName", ConfigurationSet["global-account"]);
            RestRequestBody.AddHeader("X-Events-API-Key", ConfigurationSet["api-key"]);
            RestRequestBody.AddHeader("Content-type", "application/vnd.appd.events+json;v=2");
            RestRequestBody.AddHeader("Accept", "application/vnd.appd.events+json;v=2");
            RestRequestBody.AddParameter("application/vnd.appd.events+json;v=2", "[" + jsonRequest.ToString().Replace("/r/n", "") + "]", ParameterType.RequestBody);
            var action = client.Execute(RestRequestBody);
            System.Threading.Thread.Sleep(10);

        }

    }
}
