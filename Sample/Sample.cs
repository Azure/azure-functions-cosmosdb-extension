// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.Sample
{
    public static class Sample
    {
        [FunctionName("Sample")]
        public static async Task RunAsync([CosmosDBMongoTrigger(
            databaseName: "db",
            collectionName: "original")]IReadOnlyList<BsonDocument> input,
            [CosmosDBMongo(
            databaseName: "db",
            collectionName: "duplicate")]IMongoCollection<BsonDocument> collection,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                foreach (var item in input) { 
                    log.LogInformation("Document " + item.ToJson());
                }

                await collection.InsertManyAsync(input.Select(changeDocument => changeDocument.AsBsonDocument["fullDocument"].AsBsonDocument));
            }
        }
    }
}
