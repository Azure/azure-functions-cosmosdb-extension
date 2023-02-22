﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor;
using Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor.Mongo;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo
{
    public class CosmosDBMongoTriggerListener : IListener
    {
        private ChangeProcessor<MongoPartition, MongoLease, BsonDocument> changeProcessor;

        public CosmosDBMongoTriggerListener(ITriggeredFunctionExecutor executor, ParameterInfo parameter, MongoCollectionReference monitoredCollection, MongoCollectionReference leaseCollection)
        {
            string id = Guid.NewGuid().ToString();

            MongoPartitioner partitioner = new MongoPartitioner(monitoredCollection.client.GetDatabase(monitoredCollection.databaseName), monitoredCollection.collectionName);
            MongoLeaseContainer leaseContainer = new(leaseCollection.client.GetDatabase(leaseCollection.databaseName).GetCollection<BsonDocument>(leaseCollection.collectionName), id);

            MongoProcessor processor = new MongoProcessor(monitoredCollection.client.GetDatabase(monitoredCollection.databaseName).GetCollection<BsonDocument>(monitoredCollection.collectionName), 
                async docs => {

                    TriggeredFunctionData data;
                    if (parameter.ParameterType == typeof(string))
                    {
                        data = new TriggeredFunctionData() { TriggerValue = BsonArray.Create(docs).ToJson() };
                    }
                    else if (parameter.ParameterType.IsGenericType && parameter.ParameterType.GenericTypeArguments[0] == typeof(string))
                    {
                        data = new TriggeredFunctionData() { TriggerValue = docs.Select(doc => doc.ToJson()) };
                    } 
                    else
                    {
                        data = new TriggeredFunctionData() { TriggerValue = docs };
                    }
                        
                    await executor.TryExecuteAsync(data, CancellationToken.None);
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