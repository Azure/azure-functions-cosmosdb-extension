// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.CosmosDb.Mongo;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(CosmosDBMongoWebJobsStartup))]
namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo
{
    public class CosmosDBMongoWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddCosmosDBMongo();
        }
    }
}
