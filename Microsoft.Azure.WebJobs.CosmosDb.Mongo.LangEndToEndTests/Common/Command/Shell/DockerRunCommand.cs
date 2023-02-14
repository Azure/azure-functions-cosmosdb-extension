// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

/* Shell Command responsible for creating running the docker container 
* containing function app images for particular language.
*/
public class DockerRunCommand : ShellCommand
{
	public DockerRunCommand(Language language)
	{
		cmd = BuildDockerStartCmd(language);
	}

	private string BuildDockerStartCmd(Language language)
	{
        //Starts the list with docker run and port specific to language
        var cmdList = new List<string>
        {
            Constants.DOCKER_RUN,
            Constants.DOCKER_PORT_FLAG,
            $"{Constants.LanguagePortMapping[language]}{Constants.COLON_7071}",
            //Adding env variable for the Storage Account
            Constants.DOCKER_ENVVAR_FLAG,
            Constants.AZURE_WEBJOBS_STORAGE,

            //Creating container with the same name as the image
            Constants.DOCKER_NAME_FLAG,
            Constants.LanguageImageMapping[language],

            //Adding the docker image name
            Constants.LanguageImageMapping[language]
        };

        return string.Join(Constants.STRINGLITERAL_SPACE_CHAR, cmdList);
	}
}