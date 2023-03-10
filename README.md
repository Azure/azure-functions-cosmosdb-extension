# Azure WebJobs CosmosDB Extensions

This repo contains binding extensions for the Azure WebJobs SDK intended for working with [Azure CosmosDB](https://azure.microsoft.com/en-us/products/cosmos-db/)'s various APIs. See the [Azure WebJobs SDK repo](https://github.com/Azure/azure-webjobs-sdk) for more information on WebJobs.

## NoSQL Api

The extension for working with the NoSQL API is located in the general [WebJobs Extension repo](https://github.com/Azure/azure-webjobs-sdk-extensions#documentdb)

## MongoDB Api

To configure the binding, add the Mongo connection string as an app setting or environment variable using the setting name `CosmosDB`. The name of the setting can be changed with the `ConnectionStringKey` property of the binding attribute.

### Reading Documents

The extension currently supports getting either a database or a collection. 

```csharp
public void Inputs(
    [CosmosDBMongo(DatabaseName)] IMongoDatabase db,
    [CosmosDBMongo(DatabaseName, CollectionName)] IMongoCollection<BsonDocument> coll)
{
    Assert.IsNotNull(db);
    Assert.IsNotNull(coll);
}
```

### Writing Documents

In this example, the `newItem` object is upserted into the `ItemCollection` collection of the `ItemDb` database.

```csharp
public static void InsertDocument(
    [QueueTrigger("sample")] QueueData trigger,
    [CosmosDBMongo("DatabaseName", "ItemCollection")] out BsonDocument newItem)
{
    newItem = new BsonDocument();
}
```

Simple C# objects can also be automatically converted into BSON. The following will write a simple document to the service.

```csharp
public static void InsertDocument(
    [QueueTrigger("sample")] QueueData trigger,
    [CosmosDBMongo("DatabaseName", "ItemCollection")] out ItemDoc newItem)
{
    newItem = new ItemDoc()
    {
        Text = "sample text"
    };
}
```

If you need more control, you can also specify a parameter of type `IMongoClient`. The following example uses MongoClient to query for all documents.

```csharp
public static void DocumentClient(
    [QueueTrigger("sample")] QueueData trigger,
    [CosmosDBMongo] IMongoClient client,
    TraceWriter log)
{
    var documents = client.getDatabase("Database").getCollection<BsonDocument>("Collection").find();

    foreach (BsonDocument d in documents)
    {
        log.Info(d);
    }
}
```

### Triggers

There is a separate attribute for writing functions which trigger on changes to a Cosmos Mongo collection. `CosmosDBMongoTrigger` has a few additional configurations available compared to the basic attribute. Because the trigger needs to keep track of which data has already been seen, it uses an additional collection. By default, it uses a collection named `leases` in the same database as the source collection. However `LeaseConnectionStringKey`, `LeaseDatabaseName` and `LeaseCollectionName` can be used if desired to place it elsewhere.

```csharp
public static void Trigger(
    [CosmosDBMongoTrigger(DatabaseName, MonitoredCollectionName)] IEnumerable<BsonDocument> docs,
    ILogger logger)
{
    foreach (BsonDocument doc in docs)
    {
        logger.LogInformation("Doc triggered");
    }
}
```

## Private Preview

Until the extension becomes available in the portal and extension bundles, it can be used by directly installing the extension. Either use whatever tool/template to create a CosmosDB Trigger and modify it as follows, or use the example projects for [C#](Sample) and [Typescript](typescript).

1. Remove the extension bundle from `host.json`
2. Run `func extensions install --package Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo --version 1.0.2`
3. Modify the `function.json` and/or annotations to 
   - `"type":"cosmosDBMongoTrigger"` 
   - `"datatype":"binary"` 
   - `createLeaseCollectionIfNotExists` can be removed
   - Rename `connectionStringSetting` to `connectionStringKey`
4. Change the `local.settings.json` value given in `connectionStringKey` with the mongodb account URI.
5. Change the function file itself to take a language specific binary type and deserialize it with the language specific BSON driver.
