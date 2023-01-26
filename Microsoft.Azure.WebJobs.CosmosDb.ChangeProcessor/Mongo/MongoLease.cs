// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Bson;

namespace Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor.Mongo
{
    public class MongoLease : ILease<BsonDocument>
    {
        private BsonDocument continuation;
        private string id;
        private DateTime timestamp;
        private string owner;

        internal MongoLease(BsonDocument leaseRecord)
        {
            this.continuation = leaseRecord["continuation"].AsBsonDocument;
            this.id = leaseRecord["_id"].AsString;
            this.timestamp = ConvertBsonTimestamp(leaseRecord["timestamp"].AsBsonTimestamp);
            this.owner = leaseRecord["owner"].AsString;
        }

        internal MongoLease(BsonDocument continuation, string id, DateTime timestamp, string owner)
        {
            this.continuation = continuation;
            this.id = id;
            this.timestamp = timestamp;
            this.owner = owner;
        }

        public BsonDocument Continuation()
        {
            return continuation;
        }

        public string Id()
        {
            return id;
        }

        public string Owner()
        {
            return owner;
        }

        public void SetContinuation(BsonDocument newContinuation)
        {
            this.continuation = newContinuation;
        }

        public void SetOwner(string owner)
        {
            this.owner = owner;
        }

        public DateTime Timestamp()
        {
            return timestamp;
        }

        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private BsonTimestamp ConvertDateTime(DateTime time)
        {
            return new BsonTimestamp((long)((time.ToUniversalTime() - epoch).TotalSeconds + 180000));
        }

        private DateTime ConvertBsonTimestamp(BsonTimestamp time)
        {
            return epoch.AddSeconds(time.Timestamp - 18000);
        }

        internal BsonDocument GetDocument()
        {
            return new BsonDocument(new Dictionary<string, BsonValue>()
            {
                { "continuation", this.continuation },
                { "_id", this.id },
                { "timestamp", ConvertDateTime(this.timestamp) },
                { "owner", this.owner }
            });
        }

        public void SetTimestamp(DateTime dateTime)
        {
            this.timestamp = dateTime;
        }
    }
}
