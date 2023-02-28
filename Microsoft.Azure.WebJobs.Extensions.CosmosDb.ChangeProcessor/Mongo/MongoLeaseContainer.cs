// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor.Mongo
{
    public class MongoLeaseContainer : ILeaseContainer<MongoPartition, MongoLease, BsonDocument>
    {
        private readonly IMongoCollection<BsonDocument> leaseCollection;
        private readonly string id;

        public MongoLeaseContainer(IMongoCollection<BsonDocument> leaseCollection, string id)
        {
            this.leaseCollection = leaseCollection;
            this.id = id;
        }

        public async Task CreateLeaseAsync(MongoPartition partition)
        {
            await leaseCollection.InsertOneAsync(new MongoLease(partition.ResumeToken, partition.ResumeToken.ToString()!, DateTime.UtcNow, "").GetDocument());
            return;
        }

        public async Task<IEnumerable<MongoLease>> GetAllLeasesAsync()
        {
            IAsyncCursor<BsonDocument> results = await leaseCollection.FindAsync(
                new BsonDocument("_id", new BsonDocument("$nin", new BsonArray(new List<string>() { "lock", "initialized" }))));
            return results.ToList().Select(doc => new MongoLease(doc));
        }

        public async Task<bool> IsInitializedAsync()
        {
            long result = await leaseCollection.CountDocumentsAsync(new BsonDocument("_id", "initialized"));
            return result > 0;
        }

        public async Task<bool> LockAsync()
        {
            await EnsureLockExists();
            UpdateResult taken = await leaseCollection.UpdateOneAsync(new BsonDocument(new Dictionary<string, string>()
            {
                { "_id", "lock" },
                { "owner", "" }
            }),
            new BsonDocument(new Dictionary<string, BsonDocument>()
            {
                { "$set", new BsonDocument("owner", this.id) }
            }));
            return taken.ModifiedCount > 0;
        }

        public async Task MarkInitializedAsync()
        {
            await this.leaseCollection.InsertOneAsync(new BsonDocument("_id", "initialized"));
        }

        private async Task EnsureLockExists()
        {
            try
            {
                await this.leaseCollection.InsertOneAsync(
                    new BsonDocument(new Dictionary<string, string>()
                        {
                            { "_id", "lock" },
                            { "owner", "" }
                        })
                );
            }
            catch (MongoWriteException e)
            {
                if (e.WriteError.Code == 11000)
                {
                    // Duplicate key error is okay, it means the lock existed
                } 
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> UnlockAsync()
        {
            UpdateResult taken = await this.leaseCollection.UpdateOneAsync(new BsonDocument(new Dictionary<string, string>()
            {
                { "_id", "lock" },
                { "owner", this.id }
            }),
            new BsonDocument(new Dictionary<string, BsonDocument>()
            {
                { "$set", new BsonDocument("owner" , "")}
            }));
            return taken.ModifiedCount > 0;
        }

        public async Task<Tuple<bool, MongoLease>> UpdateLeaseAsync(MongoLease lease)
        {
            UpdateResult result = await this.leaseCollection.UpdateOneAsync(new BsonDocument(new Dictionary<string, string>()
            {
                { "_id", lease.Id() },
                { "owner", this.id }
            }),
            new BsonDocument("$set", lease.GetDocument()));
            return new Tuple<bool, MongoLease>(result.ModifiedCount > 0, lease);
        }

        public async Task<Tuple<bool, MongoLease>> TakeLeaseAsync(MongoLease lease)
        {
            lease.SetOwner(this.id);
            lease.SetTimestamp(DateTime.Now);
            UpdateResult result = await this.leaseCollection.UpdateOneAsync(new BsonDocument(new Dictionary<string, string>()
            {
                { "_id", lease.Id() }
            }),
            new BsonDocument("$set", lease.GetDocument()));
            if (result.ModifiedCount > 0)
            {
                return new Tuple<bool, MongoLease>(true, lease);
            }
            return new Tuple<bool, MongoLease>(false, lease);
        }

        public async Task<Tuple<bool, MongoLease>> ReleaseLeaseAsync(MongoLease lease)
        {
            UpdateResult result = await this.leaseCollection.UpdateOneAsync(new BsonDocument(new Dictionary<string, string>()
            {
                { "_id", lease.Id() },
                { "owner", this.id }
            }),
            new BsonDocument("$set", new BsonDocument("owner", "")));
            if (result.ModifiedCount > 0)
            {
                lease.SetOwner("");
                return new Tuple<bool, MongoLease>(true, lease);
            }
            return new Tuple<bool, MongoLease>(false, lease);
        }
    }
}
