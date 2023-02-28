// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class CosmosDBMongoAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="databaseName">The Azure Cosmos database name.</param>
        /// <param name="containerName">The Azure Cosmos container name.</param>
        public CosmosDBMongoAttribute(string databaseName, string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="databaseName">The Azure Cosmos database name.</param>
        public CosmosDBMongoAttribute(string databaseName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CosmosDBMongoAttribute()
        {
        }

        /// <summary>
        /// Gets the name of the database to which the parameter applies.        
        /// May include binding parameters.
        /// </summary>
        [AutoResolve]
        public string? DatabaseName { get; private set; }

        /// <summary>
        /// Gets the name of the container to which the parameter applies. 
        /// May include binding parameters.
        /// </summary>
        [AutoResolve]
        public string? CollectionName { get; private set; }

        /// <summary>
        /// Gets the name of the database to which the parameter applies.        
        /// May include binding parameters.
        /// </summary>
        [AutoResolve]
        public string? ConnectionStringKey { get; private set; }
    }
}
