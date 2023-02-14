// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Tests;

public class JavaAppTest : BaseE2E, IClassFixture<JavaE2EFixture>
{
	private readonly ITestOutputHelper _output;

	public JavaAppTest(ITestOutputHelper output) : base(Language.JAVA, output)
	{
		_output = output;
	}

	[Fact]
	public async Task Java_App_Test_Single_Event()
	{
		//Generate Random Guids
		var reqMsgs = Utils.GenerateRandomMsgs(AppType.SINGLE_EVENT);

		//Create HttpRequestEntity with url and query parameters
		var httpRequestEntity =
			Utils.GenerateTestHttpRequestEntity(Constants.JAVAAPP_PORT, Constants.JAVA_SINGLE_APP_NAME,
				reqMsgs);

		//Test e2e flow with trigger httpRequestEntity and expectedOutcome
		await Test(AppType.SINGLE_EVENT, InvokeType.HTTP, httpRequestEntity, null, reqMsgs);
	}


	[Fact]
	public async Task Java_App_Test_Multi_Event()
	{
		//Generate Random Guids
		var reqMsgs = Utils.GenerateRandomMsgs(AppType.BATCH_EVENT);

		//Create HttpRequestEntity with url and query parameters
		var httpRequestEntity =
			Utils.GenerateTestHttpRequestEntity(Constants.JAVAAPP_PORT, Constants.JAVA_MULTI_APP_NAME,
				reqMsgs);

		//Test e2e flow with trigger httpRequestEntity and expectedOutcome
		await Test(AppType.BATCH_EVENT, InvokeType.HTTP, httpRequestEntity, null, reqMsgs);
	}
}