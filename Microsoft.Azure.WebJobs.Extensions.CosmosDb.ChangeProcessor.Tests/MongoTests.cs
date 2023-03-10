// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor.Mongo.Tests
{
    [TestClass]
    [TestCategory("EmulatorRequired")]
    public class MongoTests
    {
        private MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CosmosDB"));
        private static Guid guid;

        private ChangeProcessor<MongoPartition, MongoLease, BsonDocument> CreateProcessor(string collection, string id, Func<IEnumerable<BsonDocument>, Task> process, int maxPartitionCount = 4)
        {
            var monitoredCollection = client.GetDatabase("test").GetCollection<BsonDocument>(collection);

            MongoPartitioner partitioner = new MongoPartitioner(client.GetDatabase("test"), collection);
            MongoLeaseContainer leaseContainer = new MongoLeaseContainer(client.GetDatabase("test").GetCollection<BsonDocument>(collection + "-lease"), id);

            MongoProcessor processor = new MongoProcessor(monitoredCollection, process);

            var changeProcessor = new ChangeProcessor<MongoPartition, MongoLease, BsonDocument>(
                id, partitioner, leaseContainer, processor, new ProcessorOptions());
            changeProcessor.maxPartitionCount = maxPartitionCount;
            return changeProcessor;
        }

        [TestInitialize]
        public async Task Setup()
        {
            try
            {
                await client.DropDatabaseAsync("test");
            } catch (Exception ex) { }
            guid = Guid.NewGuid();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await client.GetDatabase("test").DropCollectionAsync(guid.ToString());
            await client.GetDatabase("test").DropCollectionAsync(guid.ToString() + "-lease");
        }

        [TestMethod]
        public async Task TestGetPartitions()
        {
            try
            {
                await client.GetDatabase("test").CreateCollectionAsync(guid.ToString());
            }
            catch (Exception) { }

            MongoPartitioner partitioner = new MongoPartitioner(client.GetDatabase("test"), guid.ToString());
            var partitions = await partitioner.GetPartitionsAsync();
            Assert.IsNotNull(partitions);
            Assert.AreEqual(1, partitions.Count());
        }

        [TestMethod]
        public async Task TestProcessor()
        {
            try
            {
                await client.GetDatabase("test").CreateCollectionAsync(guid.ToString());
            }
            catch (Exception) { }

            var monitoredCollection = client.GetDatabase("test").GetCollection<BsonDocument>(guid.ToString());

            int numChanges = 0;
            var changeProcessor = CreateProcessor(guid.ToString(), "testowner", async changes => Interlocked.Add(ref numChanges, changes.Count()));

            Task processing = changeProcessor.StartAsync();
            Thread.Sleep(10000);

            int numInserts = 200;
            var workload = Task.Run(() =>
            {
                for (int i = 0; i < numInserts; i++)
                {
                    monitoredCollection.InsertOne(new BsonDocument("i", i));
                }
            });
            await workload;
            Thread.Sleep(20000);

            await changeProcessor.StopAsync();
            await processing;

            Assert.AreEqual(numInserts, numChanges);
        }

        [TestMethod]
        public async Task TestSplitCollection()
        {
            //await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument("customAction", "CreateDatabase"));
            await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument(new Dictionary<string, object>()
            {
                { "customAction", "CreateCollection" },
                { "collection", guid.ToString() },
                { "shardKey", "_id" },
                { "autoScaleSettings", new BsonDocument("maxThroughput", 40000)}
            }));

            MongoPartitioner partitioner = new MongoPartitioner(client.GetDatabase("test"), guid.ToString());
            var partitions = await partitioner.GetPartitionsAsync();
            Assert.IsNotNull(partitions);
            Assert.AreEqual(4, partitions.Count());
        }

        [TestMethod]
        public async Task TestProcessSplitCollection()
        {
            //await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument("customAction", "CreateDatabase"));
            await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument(new Dictionary<string, object>()
            {
                { "customAction", "CreateCollection" },
                { "collection", guid.ToString() },
                { "shardKey", "_id" },
                { "autoScaleSettings", new BsonDocument("maxThroughput", 40000)}
            }));

            var monitoredCollection = client.GetDatabase("test").GetCollection<BsonDocument>(guid.ToString());

            int numChanges = 0;
            var changeProcessor = CreateProcessor(guid.ToString(), "testowner", async changes => Interlocked.Add(ref numChanges, changes.Count()));

            Task processing = changeProcessor.StartAsync();
            Thread.Sleep(10000);

            int numInserts = 500;
            var workload = Task.Run(() =>
            {
                for (int i = 0; i < numInserts; i++)
                {
                    monitoredCollection.InsertOne(new BsonDocument("i", i));
                }
            });
            await workload;
            Thread.Sleep(20000);

            await changeProcessor.StopAsync();
            await processing;

            Assert.AreEqual(numInserts, numChanges);
        }

        [TestMethod]
        public async Task TestMultipleProcessors()
        {
            await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument(new Dictionary<string, object>()
            {
                { "customAction", "CreateCollection" },
                { "collection", guid.ToString() },
                { "shardKey", "_id" },
                { "autoScaleSettings", new BsonDocument("maxThroughput", 40000)}
            }));

            var monitoredCollection = client.GetDatabase("test").GetCollection<BsonDocument>(guid.ToString());

            int numChanges = 0;
            var changeProcessor1 = CreateProcessor(guid.ToString(), "testowner1", async changes => Interlocked.Add(ref numChanges, changes.Count()), 2);
            var changeProcessor2 = CreateProcessor(guid.ToString(), "testowner2", async changes => Interlocked.Add(ref numChanges, changes.Count()), 2);

            Task processing1 = changeProcessor1.StartAsync();
            Task processing2 = changeProcessor2.StartAsync();
            Thread.Sleep(10000);

            int numInserts = 200;
            var workload = Task.Run(() =>
            {
                for (int i = 0; i < numInserts; i++)
                {
                    monitoredCollection.InsertOne(new BsonDocument("i", i));
                }
            });
            await workload;
            Thread.Sleep(20000);

            await changeProcessor1.StopAsync();
            await changeProcessor2.StopAsync();
            await processing1;
            await processing2;

            Assert.AreEqual(numInserts, numChanges);
        }


        [TestMethod]
        public async Task TestStopStartProcessor()
        {
            //await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument("customAction", "CreateDatabase"));
            await client.GetDatabase("test").RunCommandAsync<BsonDocument>(new BsonDocument(new Dictionary<string, object>()
            {
                { "customAction", "CreateCollection" },
                { "collection", guid.ToString() },
                { "shardKey", "_id" },
            }));

            var monitoredCollection = client.GetDatabase("test").GetCollection<BsonDocument>(guid.ToString());

            int numChanges = 0;
            ConcurrentDictionary<string, string> changes = new ConcurrentDictionary<string, string>();
            var changeProcessor = CreateProcessor(guid.ToString(), "testowner1", 
                async changeBatch => {
                    Interlocked.Add(ref numChanges, changeBatch.Count());
                    foreach (var change in changeBatch)
                    {
                        changes.TryAdd(change.ToJson(), "");
                    }
                });

            Task processing = changeProcessor.StartAsync();
            Thread.Sleep(10000);

            int numInserts = 500;
            var workload = Task.Run(() =>
            {
                for (int i = 0; i < numInserts; i++)
                {
                    monitoredCollection.InsertOne(new BsonDocument("i", i));
                }
            });
            await workload;

            await changeProcessor.StopAsync();
            await processing;

            processing = changeProcessor.StartAsync();
            Thread.Sleep(20000);
            await processing;

            Assert.AreEqual(numInserts, changes.Count);
        }
    }
}