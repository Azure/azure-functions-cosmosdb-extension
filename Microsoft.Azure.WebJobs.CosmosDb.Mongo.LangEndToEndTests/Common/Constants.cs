// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests.Common;

// Collection of test Constants
public static class Constants
{
	public const string HTTP_GET = "GET";
	public const string HTTP_POST = "POST";
	public const string HTTP_PUT = "PUT";
	public const string HTTP_DELETE = "DELETE";

	public const int BATCH_MESSAGE_COUNT = 3;
	public const int SINGLE_MESSAGE_COUNT = 1;

	public const string DOTNETISOLATED = "dotnet-isolated";

	public const string DOCKER_RUN = "docker run";
	public const string DOCKER_KILL = "docker rm -f";
	public const string DOCKER_PORT_FLAG = "-p";
	public const string COLON_7071 = ":7071";
	public const string DOCKER_ENVVAR_FLAG = "-e";
	public const string DOCKER_NAME_FLAG = "--name";

	public const string AZURE_WEBJOBS_STORAGE = "AzureWebJobsStorage";
	public const string AZURE_CLIENT_ID = "AZURE_CLIENT_ID";
	public const string AZURE_CLIENT_SECRET = "AZURE_CLIENT_SECRET";
	public const string AZURE_TENANT_ID = "AZURE_TENANT_ID";
	public const string AZURE_SUBSCRIPTION_ID = "AZURE_SUBSCRIPTION_ID";

	public const string STRINGLITERAL_SPACE_CHAR = " ";
	public const string STRINGLITERAL_E2E = "e2e";
	public const string STRINGLITERAL_KAFKA = "mongo";
	public const string FUNC_START = "func start";
	public const string STRINGLITERAL_HIPHEN = "-";
	public const string RESOURCE_GROUP = "EventHubRG";
	public const string EVENTHUB_NAMESPACE = "mongoextension";
	public const string STRINGLITERAL_SINGLE = "single";
	public const string STRINGLITERAL_MULTI = "multi";

	public const string PYTHONAPP_CONFLUENT_PORT = "55701";
	public const string PYTHONAPP_PORT = "51701";
	public const string PYTHONAPP_IMAGE = "azure-functions-mongo-python";
	public const string PYTHON_SINGLE_APP_NAME = "SingleHttpTriggerMongoOutput";
	public const string PYTHON_MULTI_APP_NAME = "MultiHttpTriggerMongoOutput";
	public const string PYTHONAPP_WORKER_RUNTIME = "python";

	public const string DOTNETAPP_CONFLUENT_PORT = "";
	public const string DOTNETAPP_PORT = "";
	public const string DOTNETAPP_CONFLUENT_IMAGE = "";
	public const string DOTNETAPP_IMAGE = "";
	public const string DOTNET_SINGLE_APP_NAME = "";
	public const string DOTNET_MULTI_APP_NAME = "";
	public const string DOTNET_WORKER_RUNTIME = "dotnet";

	public const string DOTNETWORKERAPP_PORT = "59200";
	public const string DOTNETWORKERAPP_IMAGE = "azure-functions-mongo-dotnet-isolated";
	public const string DOTNETWORKER_SINGLE_APP_NAME = "SingleHttpTriggerMongoOutput";
	public const string DOTNETWORKER_MULTI_APP_NAME = "MultiHttpTriggerMongoOutput";
	public const string DOTNETWORKER_WORKER_RUNTIME = "dotnet-isolated";

    public const string PWSHELL_PORT = "59501";
    public const string PWSHELL_IMAGE = "azure-functions-mongo-powershell";
	public const string PWSHELL_SINGLE_APP_NAME = "SingleHttpTriggerMongoOutput";
	public const string PWSHELL_MULTI_APP_NAME = "MultiHttpTriggerMongoOutput";
	public const string PWSHELL_WORKER_RUNTIME = "powershell";

	public const string JAVAAPP_PORT = "51651";
	public const string JAVAAPP_IMAGE = "azure-functions-mongo-java";
	public const string JAVA_SINGLE_APP_NAME = "SingleHttpTriggerMongoOutput";
	public const string JAVA_MULTI_APP_NAME = "MultiHttpTriggerMongoOutput";
	public const string JAVA_WORKER_RUNTIME = "java";

	public const string JSAPP_PORT = "51300";
	public const string JSAPP_IMAGE = "azure-functions-mongo-javascript";
	public const string JS_SINGLE_APP_NAME = "SingleHttpTriggerMongoOutput";
	public const string JS_MULTI_APP_NAME = "MultiHttpTriggerMongoOutput";
	public const string JS_WORKER_RUNTIME = "node";

	public const string TSAPP_PORT = "51452";
	public const string TSAPP_IMAGE = "azure-functions-mongo-typescript";
	public const string TS_SINGLE_APP_NAME = "SingleHttpTriggerMongoOutput";
	public const string TS_MULTI_APP_NAME = "MultiHttpTriggerMongoOutput";
	public const string TS_WORKER_RUNTIME = "node";

	public static Dictionary<Language, string> LanguagePortMapping = new()
	{
		{ Language.PYTHON, PYTHONAPP_PORT },
		{ Language.DOTNET, DOTNETAPP_PORT },
		{ Language.DOTNETISOLATED, DOTNETWORKERAPP_PORT },
		{ Language.POWERSHELL, PWSHELL_PORT },
		{ Language.JAVA, JAVAAPP_PORT },
		{ Language.JAVASCRIPT, JSAPP_PORT },
		{ Language.TYPESCRIPT, TSAPP_PORT }
	};

	public static Dictionary<Language, string> LanguageImageMapping = new()
	{
		{ Language.PYTHON, PYTHONAPP_IMAGE },
		{ Language.DOTNET, DOTNETAPP_IMAGE },
		{ Language.DOTNETISOLATED, DOTNETWORKERAPP_IMAGE },
		{ Language.POWERSHELL, PWSHELL_IMAGE },
		{ Language.JAVA, JAVAAPP_IMAGE },
		{ Language.JAVASCRIPT, JSAPP_IMAGE },
		{ Language.TYPESCRIPT, TSAPP_IMAGE }
	};

	public static Dictionary<Language, string> LanguageRuntimeMapping = new()
	{
		{ Language.PYTHON, PYTHONAPP_WORKER_RUNTIME },
		{ Language.DOTNET, DOTNET_WORKER_RUNTIME },
		{ Language.DOTNETISOLATED, DOTNETWORKER_WORKER_RUNTIME },
		{ Language.POWERSHELL, PWSHELL_WORKER_RUNTIME },
		{ Language.JAVA, JAVA_WORKER_RUNTIME },
		{ Language.JAVASCRIPT, JS_WORKER_RUNTIME },
		{ Language.TYPESCRIPT, TS_WORKER_RUNTIME }
	};

	public static List<string> IndexQueryParamMapping = new() { "message", "message1", "message2" };
}