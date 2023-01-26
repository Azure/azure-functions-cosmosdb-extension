// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Microsoft.Azure.WebJobs.CosmosDb.ChangeProcessor
{
    internal static class Trace
    {
        private static TraceSource traceSource = new TraceSource("CosmosChangeProcessor");

        public static void Information(string message, params object[] args)
        {
            traceSource.TraceEvent(TraceEventType.Information, 0, message, args);
        }
    }
}
