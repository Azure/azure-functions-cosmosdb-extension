// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor.LoadBalancing
{
    internal abstract class LoadBalancingStrategy<TLease, TContinuation>
        where TLease : ILease<TContinuation>
    {
        /// <summary>
        /// Select leases that should be taken for processing.
        /// This method will be called periodically with <see cref="ChangeFeedLeaseOptions.LeaseAcquireInterval"/>
        /// </summary>
        /// <param name="allLeases">All leases</param>
        /// <returns>Leases that should be taken for processing by this host</returns>
        public abstract IEnumerable<TLease> SelectLeasesToTake(IEnumerable<TLease> allLeases);
    }
}