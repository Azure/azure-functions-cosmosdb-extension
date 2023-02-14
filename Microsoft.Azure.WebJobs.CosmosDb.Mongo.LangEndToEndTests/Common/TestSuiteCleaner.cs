// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

/* Responsible for all cleanup after the test suite runs -
* Kills the running docker containers
* Kills all the processes created
* Cleans up the used Azure Resources
*/
public class TestSuiteCleaner
{
	public async Task CleanupTestSuiteAsync(Language language)
	{
		//Kill all docker containers
		await KillFunctionDockersAsync(language);
		ProcessLifecycleManager.GetInstance().Dispose();
		await CleanupAzureResourcesAsync(language);
	}

	private async Task KillFunctionDockersAsync(Language language)
	{
		IExecutableCommand<Process> command =
			ShellCommandFactory.CreateShellCommand(ShellCommandType.DOCKER_KILL, language);
		IExecutor<IExecutableCommand<Process>, Process> executor = new ShellCommandExecutor();
		await executor.ExecuteAsync(command);
	}

	private async Task CleanupAzureResourcesAsync(Language language)
	{
		var taskList = new List<Task>();
		await Task.WhenAll(taskList);
	}

}