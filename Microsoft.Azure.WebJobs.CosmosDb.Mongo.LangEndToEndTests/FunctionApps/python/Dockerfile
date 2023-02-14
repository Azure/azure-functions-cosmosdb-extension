FROM mcr.microsoft.com/azure-functions/python:4-python3.9-core-tools

# Copy Local File
RUN mkdir /workspace
COPY . /workspace
WORKDIR /workspace

ENV LD_LIBRARY_PATH=/workspace/test/Microsoft.Azure.WebJobs.CosmosDb.Mongo.LangEndToEndTests/FunctionApps/python/bin/runtimes/linux-x64/native

RUN python -m venv .venv
RUN pip install -r requirements.txt
RUN dotnet nuget add source /workspace/temp --name local && dotnet nuget list source
RUN func extensions install --package Microsoft.Azure.WebJobs.CosmosDb.Mongo --version 100.100.100-pre

ENTRYPOINT [ "/bin/bash", "./start.sh" ]
