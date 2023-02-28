// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo
{
    internal class CosmosDBMongoAsyncCollector<T> : IAsyncCollector<T>
    {
        private readonly CosmosDBMongoAttribute attribute;
        private readonly MongoCollectionReference reference;

        public CosmosDBMongoAsyncCollector(CosmosDBMongoAttribute attribute, MongoCollectionReference reference)
        {
            this.attribute = attribute;
            this.reference = reference;
        }

        public async Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            var db = reference.client.GetDatabase(reference.databaseName);
            var coll = db.GetCollection<T>(reference.collectionName);
            await coll.InsertOneAsync(item, null, cancellationToken);
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
        }
    }
    internal class CosmosDBMongoAsyncCollectorBuilder<T> : IConverter<CosmosDBMongoAttribute, IAsyncCollector<T>>
    {
        private readonly CosmosDBMongoExtensionConfigProvider _configProvider;

        public CosmosDBMongoAsyncCollectorBuilder(CosmosDBMongoExtensionConfigProvider config)
        {
            this._configProvider = config;
        }

        public IAsyncCollector<T> Convert(CosmosDBMongoAttribute input)
        {
            return new CosmosDBMongoAsyncCollector<T>(input, _configProvider.ResolveCollectionReference(input));
        }
    }
}
