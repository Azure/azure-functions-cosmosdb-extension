// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor
{
    public interface IPartitioner<TPartition> 
        where TPartition : IPartition
    {
        Task<IEnumerable<TPartition>> GetPartitionsAsync();
    }
}
