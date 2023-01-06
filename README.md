# Introduction
Sample console application to interactively sign in user for the microsoft.com tenant. Once the user is 
authenticated, the application calls Azure DevOps API to retrieve a list of projects from a specified 
organization supplied in appsettings.json.

# Option1: Execute exe from Visual Studio
1. Build console application in Visual Studio after cloning the repository
2. By default it uses my ADO organization, you can change which organization by updating appsettings.json

# Option2: Execute exe from command line
1. Download the latest release from this repo
2. Extract the binaries zip folder and execute the exe from command line.

# References
Refer to https://github.com/microsoft/azure-devops-auth-samples for more examples on Azure DevOps authentcation
