﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor
{
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
