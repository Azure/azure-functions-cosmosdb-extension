// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using MongoDB.Bson.Serialization;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.Tests
{
    // Triggers are difficult to mock, so test against a 'real' mongodb endpoint. Requires an environment variable CosmosDB to be set to a MongoDB connection string
    [TestClass]
    [TestCategory("EmulatorRequired")]
    public class EndToEndTests
    {
        private const string DatabaseName = "TestDatabase";
        private const string CollectionName = "TestCollection";
        private const string MonitoredCollectionName = CollectionName + "Trigger";
        private string ConnectionString = Environment.GetEnvironmentVariable("CosmosDB")!;
        private readonly TestLoggerProvider _loggerProvider = new TestLoggerProvider();

        [TestInitialize]
        public async Task Setup()
        {
            try
            {
                await new MongoClient(ConnectionString)
                    .DropDatabaseAsync(DatabaseName);
            }
            catch (MongoException ex) { }

            await new MongoClient(ConnectionString)
                .GetDatabase(DatabaseName).CreateCollectionAsync(MonitoredCollectionName);
        }   

        [TestCleanup]
        public async Task Cleanup()
        {
            await new MongoClient(ConnectionString)
                .DropDatabaseAsync(DatabaseName);
        }

        [TestMethod]
        public async Task TestTrigger()
        {
            var db = new MongoClient(ConnectionString)
                .GetDatabase(DatabaseName);
            var coll = db.GetCollection<BsonDocument>(MonitoredCollectionName);

            await RunTestAsync(async _ =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await coll.InsertOneAsync(new BsonDocument());
                }

                await WaitForPredicate(
                    () => {
                        return _loggerProvider.GetAllLogMessages().Count(m => m.FormattedMessage != null && m.FormattedMessage.Contains("Doc triggered")) == 3
                            && _loggerProvider.GetAllLogMessages().Count(m => m.FormattedMessage != null && m.FormattedMessage.Contains("Bytes triggered")) == 3;
                    });
            });
        }

        private async Task RunTestAsync(Func<IHost, Task> action)
        {
            ExplicitTypeLocator typeLocator = new ExplicitTypeLocator(typeof(TestFunctions));
            IHost host = new HostBuilder()
                .ConfigureWebJobs(builder =>
                {
                    builder.AddCosmosDBMongo();
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ITypeLocator>(typeLocator);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(_loggerProvider);
                })
            .Build();

            await host.StartAsync();
            await action(host);
            await host.StopAsync();
        }

        public static Task WaitForPredicate(Func<bool> condition, int timeout = 60 * 1000, int pollingInterval = 2 * 1000, bool throwWhenDebugging = false)
        {
            return WaitForPredicate(() => Task.FromResult(condition()), timeout, pollingInterval, throwWhenDebugging);
        }

        public static async Task WaitForPredicate(Func<Task<bool>> condition, int timeout = 60 * 1000, int pollingInterval = 2 * 1000, bool throwWhenDebugging = false)
        {
            DateTime start = DateTime.Now;
            while (!await condition())
            {
                await Task.Delay(pollingInterval);

                bool shouldThrow = !Debugger.IsAttached || (Debugger.IsAttached && throwWhenDebugging);
                if (shouldThrow && (DateTime.Now - start).TotalMilliseconds > timeout)
                {
                    throw new ApplicationException("Condition not reached within timeout.");
                }
            }
        }

        public class TestFunctions
        {
            public static void Trigger(
                [CosmosDBMongoTrigger(DatabaseName, MonitoredCollectionName)] IEnumerable<BsonDocument> docs,
                ILogger logger)
            {
                foreach (BsonDocument doc in docs)
                {
                    logger.LogInformation("Doc triggered");
                }
            }


            public static void TriggerBytes(
                [CosmosDBMongoTrigger(DatabaseName, MonitoredCollectionName, LeaseCollectionName = "leaseBytes")] byte[] bytes,
                ILogger logger)
            {
                BsonDocument result = BsonSerializer.Deserialize<BsonDocument>(bytes);
                Assert.IsNotNull(result);
                Assert.IsTrue(result["results"].IsBsonArray);
                foreach (BsonDocument doc in result["results"].AsBsonArray)
                {
                    logger.LogInformation("Bytes triggered");
                }
            }
        }
    }
}