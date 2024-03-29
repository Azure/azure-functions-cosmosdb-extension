﻿using Newtonsoft.Json;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo.Tests.models
{
    public class Item
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        public string? Text { get; set; }
    }
}
