// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor
{
    public class ProcessorOptions
    {
        public TimeSpan PollDelay { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(60);
    }
}