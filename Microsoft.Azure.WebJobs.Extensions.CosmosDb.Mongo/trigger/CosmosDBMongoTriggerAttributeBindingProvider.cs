// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System.Reflection;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo
{
    internal class CosmosDBMongoTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly CosmosDBMongoExtensionConfigProvider _configProvider;
        private readonly INameResolver _nameResolver;

        public CosmosDBMongoTriggerAttributeBindingProvider(
            CosmosDBMongoExtensionConfigProvider configProvider,
            INameResolver nameResolver)
        {
            _configProvider = configProvider;
            _nameResolver = nameResolver;
        }

        public Task<ITriggerBinding?> TryCreateAsync(TriggerBindingProviderContext context)
        {
            var attribute = context.Parameter.GetCustomAttribute<CosmosDBMongoTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding?>(null);
            }

            string connectionString = _configProvider.ResolveConnectionString(attribute.ConnectionStringKey);
            string leaseConnectionString = _configProvider.ResolveConnectionString(attribute.LeaseConnectionStringKey);

            return Task.FromResult((ITriggerBinding?)new CosmosDBMongoTriggerBinding(
                context.Parameter,
                new MongoCollectionReference(
                    _configProvider.GetService(connectionString),
                    ResolveAttributeValue(attribute.DatabaseName),
                    ResolveAttributeValue(attribute.CollectionName)
                ),
                new MongoCollectionReference(
                    _configProvider.GetService(leaseConnectionString),
                    ResolveAttributeValue(attribute.LeaseDatabaseName),
                    ResolveAttributeValue(attribute.LeaseCollectionName)
                )));
        }

        private string ResolveAttributeValue(string value)
        {
            return _nameResolver.Resolve(value) ?? value;
        }
    }
}
