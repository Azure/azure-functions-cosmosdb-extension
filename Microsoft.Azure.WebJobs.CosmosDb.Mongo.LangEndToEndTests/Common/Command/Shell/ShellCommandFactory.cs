// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

// Static Factory class to create supported Shell Commands
public static class ShellCommandFactory
{
	public static ShellCommand CreateShellCommand(ShellCommandType shellCommandType,
		Language language)
	{
		switch (shellCommandType)
		{
			case ShellCommandType.DOCKER_RUN:
				return new DockerRunCommand(language);
			case ShellCommandType.DOCKER_KILL:
				return new DockerKillCommand(language);
			default:
				throw new NotImplementedException();
		}
	}
}