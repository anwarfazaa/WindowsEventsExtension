using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThinLogParser.Objects;

namespace ThinLogParser
{
    public static class ControllerCommunication
    {
        private static readonly Dictionary<string, string> Config = ConfigurationReader.LoadXmlConfiguration();

        private static RestClient CreateClient(string endpointSuffix)
        {
            return new RestClient($"{Config["event-service-endpoint"]}{endpointSuffix}");
        }

        private static RestRequest CreateRequest(Method method)
        {
            var request = new RestRequest(method);
            request.AddHeader("X-Events-API-AccountName", Config["global-account"]);
            request.AddHeader("X-Events-API-Key", Config["api-key"]);
            request.AddHeader("Content-type", "application/vnd.appd.events+json;v=2");
            request.AddHeader("Accept", "application/vnd.appd.events+json;v=2");
            return request;
        }

        public static string CreateSchema()
        {
            var client = CreateClient($"/events/schema/{Config["events-schema-name"]}");
            var request = CreateRequest(Method.POST);

            var schema = new JObject
            {
                ["schema"] = new JObject
                {
                    ["LoggedTime"] = "date",
                    ["LogName"] = "string",
                    ["EventLevel"] = "string",
                    ["Hostname"] = "string",
                    ["Source"] = "string",
                    ["Message"] = "string",
                    ["EventID"] = "float"
                }
            };

            request.AddParameter("application/vnd.appd.events+json;v=2", schema.ToString(), ParameterType.RequestBody);
            var response = client.Execute(request);
            return response.StatusDescription;
        }

        public static string DeleteSchema()
        {
            var client = CreateClient($"/events/schema/{Config["events-schema-name"]}");
            var request = CreateRequest(Method.DELETE);
            var response = client.Execute(request);
            return response.StatusDescription;
        }

        public static async Task PublishEventsAsync(List<Event> events)
        {
            const int maxBatchSize = 5;
            const int maxSizeBytes = 20 * 1024;

            var batch = new JArray();
            int currentSize = 0;

            foreach (var evt in events)
            {
                var eventObj = new JObject
                {
                    ["LoggedTime"] = evt.LoggedTime,
                    ["LogName"] = evt.LogName,
                    ["EventLevel"] = evt.EventLevel,
                    ["Hostname"] = evt.HostName,
                    ["Source"] = evt.Source,
                    ["Message"] = evt.Message,
                    ["EventID"] = evt.EventID
                };

                batch.Add(eventObj);
                currentSize += Encoding.UTF8.GetByteCount(eventObj.ToString());

                if (batch.Count >= maxBatchSize || currentSize >= maxSizeBytes)
                {
                    await PushBatchAsync(batch);
                    batch.Clear();
                    currentSize = 0;
                }
            }

            if (batch.Count > 0)
            {
                await PushBatchAsync(batch);
            }
        }

        private static async Task PushBatchAsync(JArray batch)
        {
            var client = CreateClient($"/events/publish/{Config["events-schema-name"]}");
            var request = CreateRequest(Method.POST);

            request.AddParameter("application/vnd.appd.events+json;v=2", batch.ToString(), ParameterType.RequestBody);
            await client.ExecuteAsync(request);
            await Task.Delay(10); // non-blocking equivalent to Thread.Sleep(10)
        }
    }
}
