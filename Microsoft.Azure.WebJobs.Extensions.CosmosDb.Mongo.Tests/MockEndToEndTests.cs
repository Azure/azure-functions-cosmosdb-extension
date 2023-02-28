// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo.Tests.models;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo.Tests
{
    [TestClass]
    public class MockEndToEndTests
    {
        private const string DatabaseName = "TestDatabase";
        private const string CollectionName = "TestCollection";

        private Mock<IMongoCollection<T>> CreateMockCollection<T>()
        {
            var mock = new Mock<IMongoCollection<T>>(MockBehavior.Strict);
            mock
                .Setup(m => m.InsertOneAsync(It.IsAny<T>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            return mock;
        }

        private (Mock<ICosmosDBMongoServiceFactory>, IEnumerable<dynamic>) CreateMocks()
        {
            var monitoredDatabaseMock = new Mock<IMongoDatabase>(MockBehavior.Strict);

            var bsonMock = CreateMockCollection<BsonDocument>();
            monitoredDatabaseMock
                .Setup(m => m.GetCollection<BsonDocument>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(bsonMock.Object);

            var itemMock = CreateMockCollection<Item>();
            monitoredDatabaseMock
                .Setup(m => m.GetCollection<Item>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(itemMock.Object);

            var serviceMock = new Mock<IMongoClient>(MockBehavior.Strict);
            serviceMock.Setup(m => m.GetDatabase(It.IsAny<string>(), default)).Returns(monitoredDatabaseMock.Object);

            var factoryMock = new Mock<ICosmosDBMongoServiceFactory>(MockBehavior.Strict);
            factoryMock.Setup(f => f.CreateService(It.IsAny<String>())).Returns(serviceMock.Object);

            return (factoryMock, new List<dynamic> { monitoredDatabaseMock, bsonMock, itemMock, serviceMock }.AsEnumerable());
        }

        [TestMethod]
        public async Task TestClient()
        {
            var (factoryMock, mocks) = CreateMocks();
            await RunTestAsync(factoryMock.Object, "Client");
        }

        [TestMethod]
        public async Task TestCollector()
        {
            var (factoryMock, mocks) = CreateMocks();
            await RunTestAsync(factoryMock.Object, "Collector");
        }

        [TestMethod]
        public async Task TestOutputs()
        {
            var (factoryMock, mocks) = CreateMocks();
            await RunTestAsync(factoryMock.Object, "Outputs");
        }

        [TestMethod]
        public async Task TestInputs()
        {
            var (factoryMock, mocks) = CreateMocks();
            await RunTestAsync(factoryMock.Object, "Inputs");
        }

        private async Task RunTestAsync(ICosmosDBMongoServiceFactory factory, string testName)
        {
            ExplicitTypeLocator typeLocator = new ExplicitTypeLocator(typeof(TestFunctions));
            IHost host = new HostBuilder()
                .ConfigureWebJobs(builder =>
                {
                    builder.AddCosmosDBMongo();
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ICosmosDBMongoServiceFactory>(factory);
                    services.AddSingleton<ITypeLocator>(typeLocator);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
            .Build();

            await host.StartAsync();
            await ((JobHost)host.Services.GetService<IJobHost>()).CallAsync(typeof(TestFunctions).GetMethod(testName), null);
            await host.StopAsync();
        }

        public class TestFunctions
        {
            [NoAutomaticTrigger()]
            public static void Client(
                [CosmosDBMongo] IMongoClient client)
            {
                Assert.IsNotNull(client);
            }

            [NoAutomaticTrigger()]
            public async Task Collector(
                [CosmosDBMongo(DatabaseName, CollectionName)] IAsyncCollector<BsonDocument> collector,
                [CosmosDBMongo(DatabaseName, CollectionName)] IAsyncCollector<Item> itemCollector,
                [CosmosDBMongo(DatabaseName, CollectionName)] ICollector<BsonDocument> syncCollector,
                [CosmosDBMongo(DatabaseName, CollectionName)] ICollector<Item> syncItemCollector)
            {
                for (int i = 0; i < 3; i++)
                {
                    await collector.AddAsync(new BsonDocument());
                    await itemCollector.AddAsync(new Item());
                    syncCollector.Add(new BsonDocument());
                    syncItemCollector.Add(new Item());
                }
            }


            [NoAutomaticTrigger()]
            public void Outputs(
                [CosmosDBMongo(DatabaseName, CollectionName)] out BsonDocument doc,
                [CosmosDBMongo(DatabaseName, CollectionName)] out BsonDocument[] docs,
                [CosmosDBMongo(DatabaseName, CollectionName)] out Item obj,
                [CosmosDBMongo(DatabaseName, CollectionName)] out Item[] items)
            {
                doc = new BsonDocument();
                docs = new BsonDocument[1] { new BsonDocument() };

                obj = new Item();
                items = new Item[1] { new Item() };
            }

            [NoAutomaticTrigger()]
            public void Inputs(
                [CosmosDBMongo(DatabaseName)] IMongoDatabase db,
                [CosmosDBMongo(DatabaseName, CollectionName)] IMongoCollection<BsonDocument> coll)
            {
                Assert.IsNotNull(db);
                Assert.IsNotNull(coll);
            }
        }
    }
}