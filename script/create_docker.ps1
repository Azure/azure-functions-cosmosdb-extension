# Build docker images
pushd  "..\Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests\FunctionApps\java"
docker build -t azure-functions-mongo-java .
popd
pushd  "..\Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests\FunctionApps\python"
docker build -t azure-functions-mongo-python .
popd