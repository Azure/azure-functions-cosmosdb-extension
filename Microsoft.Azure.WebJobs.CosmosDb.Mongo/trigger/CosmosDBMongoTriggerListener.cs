// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor;
using Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor.Mongo;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo
{
    public class CosmosDBMongoTriggerListener : IListener
    {
        private ChangeProcessor<MongoPartition, MongoLease, BsonDocument> changeProcessor;

        public CosmosDBMongoTriggerListener(ITriggeredFunctionExecutor executor, MongoCollectionReference monitoredCollection, MongoCollectionReference leaseCollection)
        {
            string id = Guid.NewGuid().ToString();

            MongoPartitioner partitioner = new MongoPartitioner(monitoredCollection.client.GetDatabase(monitoredCollection.databaseName), monitoredCollection.collectionName);
            MongoLeaseContainer leaseContainer = new(leaseCollection.client.GetDatabase(leaseCollection.databaseName).GetCollection<BsonDocument>(leaseCollection.collectionName), id);

            MongoProcessor processor = new MongoProcessor(monitoredCollection.client.GetDatabase(monitoredCollection.databaseName).GetCollection<BsonDocument>(monitoredCollection.collectionName), 
                async docs => {
                    await executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = docs }, CancellationToken.None);
                });

            this.changeProcessor = new ChangeProcessor<MongoPartition, MongoLease, BsonDocument>(
                id, partitioner, leaseContainer, processor, new ProcessorOptions());
        }

        public void Cancel()    
        {
            this.StopAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.changeProcessor.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.changeProcessor.StopAsync();
        }
    }
}