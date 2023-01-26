using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.Tests.models
{
    public class Item
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        public string? Text { get; set; }
    }
}
