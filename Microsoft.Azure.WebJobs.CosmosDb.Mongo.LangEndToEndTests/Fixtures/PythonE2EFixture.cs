// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

// Common fixture for all test cases for Python using Eventhub as kafka provider
public class PythonE2EFixture : MongoE2EFixture
{
	public PythonE2EFixture() : base(Language.PYTHON) { }
}