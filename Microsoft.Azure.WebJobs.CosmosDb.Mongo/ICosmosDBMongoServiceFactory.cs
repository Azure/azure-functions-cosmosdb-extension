// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo
{
    public interface ICosmosDBMongoServiceFactory
    {
        public IMongoClient CreateService(string connection);
    }
}