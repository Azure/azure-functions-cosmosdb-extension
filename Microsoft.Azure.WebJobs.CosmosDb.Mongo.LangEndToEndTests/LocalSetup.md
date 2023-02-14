## Software Installation

Function apps for the test run in docker containers. Please install docker on the system.
From the root/script repository, run `create_docker.ps1` to create function app docker images

## Additional Infrastructure Resources

For the tests to run end-to-end we require a cosmosdd connection string set in `CosmosDb` environment variable.
