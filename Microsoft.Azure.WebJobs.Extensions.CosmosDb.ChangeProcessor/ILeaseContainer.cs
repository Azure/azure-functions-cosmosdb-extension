// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILeaseContainer<TPartition, TLease, TContinuation>
        where TPartition : IPartition
        where TLease : ILease<TContinuation>
    {
        public Task<bool> LockAsync();
        public Task<bool> UnlockAsync();
        public Task<bool> IsInitializedAsync();
        public Task MarkInitializedAsync();

        public Task<IEnumerable<TLease>> GetAllLeasesAsync();

        public Task CreateLeaseAsync(TPartition partition);

        public Task<Tuple<bool, TLease>> TakeLeaseAsync(TLease lease);

        public Task<Tuple<bool, TLease>> ReleaseLeaseAsync(TLease lease);

        public Task<Tuple<bool, TLease>> UpdateLeaseAsync(TLease lease);
    }
}