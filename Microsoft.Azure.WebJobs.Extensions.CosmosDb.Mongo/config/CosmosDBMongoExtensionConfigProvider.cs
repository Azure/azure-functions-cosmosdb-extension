// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo
{
    internal class CosmosDBMongoExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly ICosmosDBMongoServiceFactory _serviceFactory;
        private readonly IConfiguration _configuration;
        private readonly INameResolver _nameResolver;

        public CosmosDBMongoExtensionConfigProvider(
            ICosmosDBMongoServiceFactory serviceFactory,
            IConfiguration configuration,
            INameResolver nameResolver)
        {
            _serviceFactory = serviceFactory;
            _configuration = configuration;
            _nameResolver = nameResolver;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            var triggerRule = context.AddBindingRule<CosmosDBMongoTriggerAttribute>();
            triggerRule.AddValidator((attr, t) =>
            {
                if (string.IsNullOrEmpty(ResolveConnectionString(attr.ConnectionStringKey)))
                {
                    throw new InvalidOperationException("Connection string setting must be provided in an app setting or environment variable.");
                }
            });
            triggerRule.BindToTrigger(new CosmosDBMongoTriggerAttributeBindingProvider(this, _nameResolver));

            var rule = context.AddBindingRule<CosmosDBMongoAttribute>();
            rule.AddValidator((attr, t) =>
            {
                if (string.IsNullOrEmpty(ResolveConnectionString(attr.ConnectionStringKey)))
                {
                    throw new InvalidOperationException("Connection string setting must be provided in an app setting or environment variable.");
                }
            });
            rule.BindToCollector<OpenType.Poco>(typeof(CosmosDBMongoAsyncCollectorBuilder<>), this);
            rule.WhenIsNull(nameof(CosmosDBMongoAttribute.DatabaseName))
            .BindToInput(attribute =>
            {
                return _serviceFactory.CreateService(ResolveConnectionString(attribute.ConnectionStringKey));
            });
            rule.WhenIsNull(nameof(CosmosDBMongoAttribute.CollectionName)).WhenIsNotNull(nameof(CosmosDBMongoAttribute.DatabaseName))
                .BindToInput(attribute =>
            {
                return _serviceFactory.CreateService(ResolveConnectionString(attribute.ConnectionStringKey)).GetDatabase(attribute.DatabaseName);
            });
            rule.WhenIsNotNull(nameof(CosmosDBMongoAttribute.CollectionName)).WhenIsNotNull(nameof(CosmosDBMongoAttribute.DatabaseName))
                .BindToInput(attribute =>
                {
                    return _serviceFactory.CreateService(ResolveConnectionString(attribute.ConnectionStringKey))
                        .GetDatabase(attribute.DatabaseName)
                        .GetCollection<BsonDocument>(attribute.CollectionName);
                });
        }

        internal IMongoClient GetService(string connection)
        {
            return _serviceFactory.CreateService(connection);
        }

        internal MongoCollectionReference ResolveCollectionReference(CosmosDBMongoAttribute attribute)
        {
            return new MongoCollectionReference(
                GetService(ResolveConnectionString(attribute.ConnectionStringKey)),
                attribute.DatabaseName!,
                attribute.CollectionName!);
        }

        public string ResolveConnectionString(string? connectionStringKey)
        {
            if (string.IsNullOrEmpty(connectionStringKey))
            {
                connectionStringKey = Constants.DefaultConnectionStringKey;
            }

            string connection = _configuration.GetConnectionString(connectionStringKey);
            if (string.IsNullOrEmpty(connection))
            {
                connection = _configuration.GetValue<string>(connectionStringKey);
            }
            if (string.IsNullOrEmpty(connection))
            {
                connection = _configuration.GetWebJobsConnectionString(connectionStringKey);
            }
            if (string.IsNullOrEmpty(connection))
            {
                throw new InvalidOperationException($"Cosmos DB connection configuration '{connectionStringKey}' does not exist. " +
                                    $"Make sure that it is a defined App Setting or environment variable.");
            }
            return connection;
        }
    }
}
