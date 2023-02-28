// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor.LoadBalancing
{
    internal sealed class EqualPartitionsBalancingStrategy<TLease, TContinuation> : LoadBalancingStrategy<TLease, TContinuation>
        where TLease : ILease<TContinuation>
        where TContinuation : notnull
    {
        internal static int DefaultMinLeaseCount = 0;
        internal static int DefaultMaxLeaseCount = 0;

        private readonly string hostName;
        private readonly int minPartitionCount;
        private readonly int maxPartitionCount;
        private readonly TimeSpan leaseExpirationInterval;

        public EqualPartitionsBalancingStrategy(string hostName, int minPartitionCount, int maxPartitionCount, TimeSpan leaseExpirationInterval)
        {
            this.hostName = hostName ?? throw new ArgumentNullException(nameof(hostName));
            this.minPartitionCount = minPartitionCount;
            this.maxPartitionCount = maxPartitionCount;
            this.leaseExpirationInterval = leaseExpirationInterval;
        }

        public override IEnumerable<TLease> SelectLeasesToTake(IEnumerable<TLease> allLeases)
        {
            Dictionary<string, int> workerToPartitionCount = new();
            List<TLease> expiredLeases = new List<TLease>();
            Dictionary<TContinuation, TLease> allPartitions = new();
            this.CategorizeLeases(allLeases, allPartitions, expiredLeases, workerToPartitionCount);

            int partitionCount = allPartitions.Count;
            int workerCount = workerToPartitionCount.Count;
            if (partitionCount <= 0)
            {
                return Enumerable.Empty<TLease>();
            }

            int target = this.CalculateTargetPartitionCount(partitionCount, workerCount);
            int myCount = workerToPartitionCount[this.hostName];
            int partitionsNeededForMe = target - myCount;

            Trace.Information(
                "Host '{0}' {1} partitions, {2} hosts, {3} available leases, target = {4}, min = {5}, max = {6}, mine = {7}, will try to take {8} lease(s) for myself'.",
                this.hostName,
                partitionCount,
                workerCount,
                expiredLeases.Count,
                target,
                this.minPartitionCount,
                this.maxPartitionCount,
                myCount,
                Math.Max(partitionsNeededForMe, 0));

            if (partitionsNeededForMe <= 0)
            {
                return Enumerable.Empty<TLease>();
            }

            if (expiredLeases.Count > 0)
            {
                return expiredLeases.Take(partitionsNeededForMe);
            }

            TLease? stolenLease = GetLeaseToSteal(workerToPartitionCount, target, partitionsNeededForMe, allPartitions);
            return stolenLease == null ? Enumerable.Empty<TLease>() : new[] { stolenLease };
        }

        private static TLease? GetLeaseToSteal(
            Dictionary<string, int> workerToPartitionCount,
            int target,
            int partitionsNeededForMe,
            Dictionary<TContinuation, TLease> allPartitions)
        {
            KeyValuePair<string, int> workerToStealFrom = FindWorkerWithMostPartitions(workerToPartitionCount);
            if (workerToStealFrom.Value > target - (partitionsNeededForMe > 1 ? 1 : 0))
            {
                return allPartitions.Values.First(partition => string.Equals(partition.Owner(), workerToStealFrom.Key, StringComparison.OrdinalIgnoreCase));
            }

            return default;
        }

        private static KeyValuePair<string, int> FindWorkerWithMostPartitions(Dictionary<string, int> workerToPartitionCount)
        {
            KeyValuePair<string, int> workerToStealFrom = default;
            foreach (KeyValuePair<string, int> kvp in workerToPartitionCount)
            {
                if (workerToStealFrom.Value <= kvp.Value)
                {
                    workerToStealFrom = kvp;
                }
            }

            return workerToStealFrom;
        }

        private int CalculateTargetPartitionCount(int partitionCount, int workerCount)
        {
            int target = 1;
            if (partitionCount > workerCount)
            {
                target = (int)Math.Ceiling((double)partitionCount / workerCount);
            }

            if (this.maxPartitionCount > 0 && target > this.maxPartitionCount)
            {
                target = this.maxPartitionCount;
            }

            if (this.minPartitionCount > 0 && target < this.minPartitionCount)
            {
                target = this.minPartitionCount;
            }

            return target;
        }

        private void CategorizeLeases(
            IEnumerable<TLease> allLeases,
            Dictionary<TContinuation, TLease> allPartitions,
            List<TLease> expiredLeases,
            Dictionary<string, int> workerToPartitionCount)
        {
            foreach (TLease lease in allLeases)
            {
                allPartitions.Add(lease.Continuation(), lease);
                if (string.IsNullOrWhiteSpace(lease.Owner()) || this.IsExpired(lease))
                {
                    Trace.Information("Found unused or expired lease: {0}", lease);
                    expiredLeases.Add(lease);
                }
                else
                {
                    string assignedTo = lease.Owner();
                    if (workerToPartitionCount.TryGetValue(assignedTo, out int count))
                    {
                        workerToPartitionCount[assignedTo] = count + 1;
                    }
                    else
                    {
                        workerToPartitionCount.Add(assignedTo, 1);
                    }
                }
            }

            if (!workerToPartitionCount.ContainsKey(this.hostName))
            {
                workerToPartitionCount.Add(this.hostName, 0);
            }
        }

        private bool IsExpired(TLease lease)
        {
            return lease.Timestamp().ToUniversalTime() + this.leaseExpirationInterval < DateTime.UtcNow;
        }
    }
}