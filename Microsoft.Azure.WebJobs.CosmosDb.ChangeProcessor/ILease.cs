// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ILease<TContinuation>
    {
        public DateTime Timestamp();
        public string Owner();
        public string Id();
        public TContinuation Continuation();
        void SetContinuation(TContinuation newContinuation);
        void SetOwner(string owner);
        void SetTimestamp(DateTime dateTime);
    }
}
