# About
Trivial web app to track gitlab popular MR comments. Web api part

## Description
tbd

## Related links
* Client: [https://github.com/bodynar/MAS.GitlabComments.Client](https://github.com/bodynar/MAS.GitlabComments.Client)

# Installation

## Requirements

tbd
* dotnet
* MSSQL server
* (_optional_) Internet Information Services (IIS)

## First-time installation

1. Download the latest the release from [release page](https://github.com/bodynar/MAS.GitlabComments.Api/releases)
> If no zip file with app content provided - download source code, build & [publish WebApp via dotnet](https://learn.microsoft.com/en-us/dotnet/core/deploying/deploy-with-cli)
2. Create a database on mssql server
3. Update `DefaultConnection` setting in `appsettings.json` file to match your database configuration
> Example: `Server=MyServer;Database=GitlabComments;User Id=user;Password=password;MultipleActiveResultSets=true`
4. Configure the rest of the application settings in the `appsettings.json` file as you wish
5. Run app
    * **dotnet**: to run use [cli command](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run)
    * **Internet Information Services (IIS)**: to run with IIS please proceed with [official guide](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-8.0)
6. To use client app proceed with [Client app installation guide](https://github.com/bodynar/MAS.GitlabComments.Client?tab=readme-ov-file#installation)

## Update
* Before updating app make sure that app is stopped.
* To update your app just follow steps 1 & 5.
* On step 1 you need to replace existed files, except `appsettings.json`
* [**_ONLY IF NO INSTRUCTIONS PROVIDED IN RELEASE_**] Compare newest version of `appsettings.json` and your version, add new lines to your settings and save the file.

## Related links
* Dotnet deploy with cli - [https://learn.microsoft.com/en-us/dotnet/core/deploying/deploy-with-cli](https://learn.microsoft.com/en-us/dotnet/core/deploying/deploy-with-cli)
* Dotnet run - [https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run)
* Host ASP.NET Core on Windows with IIS - [https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-8.0](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-8.0)
