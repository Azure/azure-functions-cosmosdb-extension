// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

/* Shell Command responsible for kill the running the docker container 
* containing function app images for particular language.
*/
public class DockerKillCommand : ShellCommand
{
	public DockerKillCommand(Language language)
	{
		cmd = BuildDockerKillCmd(language);
	}

	private string BuildDockerKillCmd(Language language)
	{
        //Starting the list with docker rm
        var cmdList = new List<string>
        {
            Constants.DOCKER_KILL,      //Adding the image name to kill
            Constants.LanguageImageMapping[language]
        };

        return string.Join(Constants.STRINGLITERAL_SPACE_CHAR, cmdList);
	}
}