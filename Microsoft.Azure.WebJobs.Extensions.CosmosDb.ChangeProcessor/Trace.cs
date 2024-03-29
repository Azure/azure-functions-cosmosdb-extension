﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Microsoft.Azure.WebJobs.Extensions.CosmosDb.ChangeProcessor
{
    internal static class Trace
    {
        private static TraceSource traceSource = new TraceSource("CosmosChangeProcessor");

        public static void Information(string id, string message, params object[] args)
        {
            traceSource.TraceEvent(TraceEventType.Information, 0, id + " - " + message, args);
        }
    }
}
