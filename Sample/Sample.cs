// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo.Sample
{
    public static class Sample
    {
        [FunctionName("Sample")]
        public static async Task RunAsync([CosmosDBMongoTrigger(
            databaseName: "db",
            collectionName: "original")]byte[] input,
            [CosmosDBMongo(
            databaseName: "db",
            collectionName: "duplicate")]IMongoCollection<BsonDocument> collection,
            ILogger log)
        {
            List<BsonValue> docs = BsonSerializer.Deserialize<BsonDocument>(input)["results"].AsBsonArray.ToList();
            log.LogInformation("Documents modified " + docs.Count);
            foreach (var item in docs) { 
                log.LogInformation("Document " + item.ToJson());
            }

            await collection.InsertManyAsync(docs.Select(changeDocument => changeDocument.AsBsonDocument["fullDocument"].AsBsonDocument));
        }
    }
}
