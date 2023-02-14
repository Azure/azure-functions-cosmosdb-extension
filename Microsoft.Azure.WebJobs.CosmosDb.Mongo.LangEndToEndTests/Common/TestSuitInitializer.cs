// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

/* Responsible for all initilisation before actual test startup -
* Creation of Azure resources - Eventhubs and Storage Queues
* Function App startup
*/
public class TestSuitInitializer
{
	private readonly ILogger _logger = TestLogger.GetTestLogger();

	public async Task InitializeTestSuitAsync(Language language)
	{
		await StartupApplicationAsync(language);
	}

	private async Task StartupApplicationAsync(Language language)
	{
		IExecutableCommand<Process> command =
			ShellCommandFactory.CreateShellCommand(ShellCommandType.DOCKER_RUN, language);
		IExecutor<IExecutableCommand<Process>, Process> executor = new ShellCommandExecutor();
		ProcessLifecycleManager.GetInstance().AddProcess(await executor.ExecuteAsync(command));
	}
}