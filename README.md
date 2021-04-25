# Introduction
Sample console application to interactively sign in user for the microsoft.com tenant. Once the user is 
authenticated, the application calls Azure DevOps API to retrieve a list of projects from a specified 
organization supplied in appsettings.json.

# How to Run Console Application
1. Build console application in Visual Studio after cloning the repository
2. By default it uses my ADO organization, you can change which organization by updating appsettings.json

# References
Refer to https://github.com/microsoft/azure-devops-auth-samples for more examples on Azure DevOps authentcation