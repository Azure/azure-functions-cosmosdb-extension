// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.Mongo
{
    internal class CosmosDBMongoTriggerBinding : ITriggerBinding
    {
        private static readonly IReadOnlyDictionary<string, Type> _emptyBindingContract = new Dictionary<string, Type>();
        private static readonly IReadOnlyDictionary<string, object> _emptyBindingData = new Dictionary<string, object>();
        private readonly ParameterInfo parameter;
        private readonly MongoCollectionReference monitoredCollection;
        private readonly MongoCollectionReference leaseCollection;

        public CosmosDBMongoTriggerBinding(ParameterInfo parameter, MongoCollectionReference monitoredCollection, MongoCollectionReference leaseCollection)
        {
            this.parameter = parameter;
            this.monitoredCollection = monitoredCollection;
            this.leaseCollection = leaseCollection;
        }

        public Type TriggerValueType => parameter.ParameterType;

        public IReadOnlyDictionary<string, Type> BindingDataContract => _emptyBindingContract;

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult<ITriggerData>(new TriggerData(new CosmosDBMongoValueProvider(value), _emptyBindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            return Task.FromResult<IListener>(new CosmosDBMongoTriggerListener(
                context.Executor,
                parameter,
                monitoredCollection,
                leaseCollection));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new TriggerParameterDescriptor()
            {
                Name = parameter.Name,
            };
        }

        private class CosmosDBMongoValueProvider : IValueProvider
        {
            private readonly object value;

            public CosmosDBMongoValueProvider(object value)
            {
                this.value = value;
            }

            public Type Type => typeof(IEnumerable<BsonDocument>);

            public Task<object> GetValueAsync()
            {
                return Task.FromResult(value);
            }

            public string ToInvokeString() => string.Empty;
        }
    }
}
