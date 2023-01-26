// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class CosmosDBMongoTriggerAttribute : Attribute
    {
        public CosmosDBMongoTriggerAttribute(string databaseName, string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Missing information for the collection to monitor", "collectionName");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Missing information for the database to monitor", "databaseName");
            }

            CollectionName = collectionName;
            DatabaseName = databaseName;
            LeaseCollectionName = "leases";
            LeaseDatabaseName = DatabaseName;
        }

        [AutoResolve]
        public string? ConnectionStringKey { get; private set; }

        [AutoResolve]
        public string DatabaseName { get; private set; }

        [AutoResolve]
        public string CollectionName { get; private set; }
        
        [ConnectionString]
        public string? LeaseConnectionStringKey { get; private set; }

        [AutoResolve]
        public string LeaseDatabaseName { get; private set; }

        [AutoResolve]
        public string LeaseCollectionName { get; private set; }
    }
}
