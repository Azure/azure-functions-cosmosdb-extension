// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo
{
    public class DefaultCosmosDBMongoServiceFactory : ICosmosDBMongoServiceFactory
    {
        public IMongoClient CreateService(string connection)
        {
            return new MongoClient(connection);
        }
    }
}
