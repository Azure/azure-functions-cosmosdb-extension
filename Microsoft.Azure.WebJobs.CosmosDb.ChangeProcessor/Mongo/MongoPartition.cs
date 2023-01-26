// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MongoDB.Bson;

namespace Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor.Mongo
{
    public class MongoPartition : IPartition
    {
        public MongoPartition(BsonDocument resumeToken)
        {
            ResumeToken = resumeToken;
        }

        public BsonDocument ResumeToken { get; }
    }
}
