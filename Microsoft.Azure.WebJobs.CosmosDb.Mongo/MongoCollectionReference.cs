// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo
{
    public class MongoCollectionReference
    {
        public IMongoClient client { get; }
        public string databaseName { get; }
        public string collectionName { get; }

        public MongoCollectionReference(IMongoClient client, string databaseName, string collectionName)
        {
            this.client = client;
            this.databaseName = databaseName;
            this.collectionName = collectionName;
        }
    }
}
