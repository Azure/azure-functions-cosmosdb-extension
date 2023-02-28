// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor.Mongo
{
    public class MongoProcessor : IProcessor<MongoLease, BsonDocument>
    {
        private readonly IMongoCollection<BsonDocument> monitoredCollection;
        private readonly Func<IEnumerable<BsonDocument>, Task> process;

        public MongoProcessor(IMongoCollection<BsonDocument> monitoredCollection, Func<IEnumerable<BsonDocument>, Task> process)
        {
            this.monitoredCollection = monitoredCollection;
            this.process = process;
        }

        private PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument> WatchPipeline()
        {
            return new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match(new BsonDocument(new Dictionary<string, BsonDocument>(){
                            {"operationType", new BsonDocument("$in", new BsonArray(new List<string>(){"insert", "update", "replace" }))}
                        }))
                .Project(new BsonDocument(new Dictionary<string, bool>()
                {
                    { "_id", true },
                    { "fullDocument", true },
                    { "ns", true },
                    { "documentKey", true }
                }));
        }

        public async Task<BsonDocument?> ProcessAsync(MongoLease lease, CancellationToken cancellationToken, Action<TimeSpan> delay, Action<BsonDocument> checkpoint)
        {
            ChangeStreamOptions options = new ChangeStreamOptions()
            {   
                ResumeAfter = lease.Continuation(),
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
                MaxAwaitTime = TimeSpan.FromSeconds(5)
            };
            IChangeStreamCursor<BsonDocument> cursor = await this.monitoredCollection.WatchAsync(WatchPipeline(), options, cancellationToken);

            // Attempt to drain change stream, with a limit to the number of attempts between dropping back to the poll interval
            for (int i = 0; i<100; i++)
            { 
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await cursor.MoveNextAsync(cancellationToken);
                if (!cursor.Current.Any())
                {
                    break;
                }
                await process(cursor.Current);
                checkpoint(cursor.GetResumeToken());
            } 

            return cursor.GetResumeToken();
        }
    }
}
