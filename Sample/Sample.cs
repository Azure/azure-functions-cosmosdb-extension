// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.CosmosDb.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.Sample
{
    public static class Sample
    {
        [FunctionName("Sample")]
        public static async Task RunAsync([CosmosDBMongoTrigger(
            databaseName: "test",
            collectionName: "test2")]IReadOnlyList<BsonDocument> input,
            [CosmosDBMongo(
            databaseName: "test",
            collectionName: "test3")]IMongoCollection<BsonDocument> collection,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document " + input[0].ToJson());

                await collection.InsertManyAsync(input);
            }
        }
    }
}
