# MAS.GitlabComments.Api

The application serves as the **backend** component of the GitlabComments application.

## Usage

The application can only be used as the backend component of a web application, i.e., as a web API.

For the client-side (UI), you need to use the [corresponding application](https://github.com/bodynar/MAS.GitlabComments.Client).

### Installation

The application can be installed using IIS or other methods that support deploying ASP.NET applications.

#### Prerequisites:
1. Database Server
- A database must exist in the DBMS where the application data will be stored.
- A user account with full access to the database must be created in the DBMS.

#### Installation Steps:
To install, download the files for the required version of the application (see the [application releases page](https://github.com/bodynar/MAS.GitlabComments.Api/releases)).

1. Execute all scripts from the `\SqlScripts` folder in the database. If errors occur, abort the installation process and contact the publisher (application author).
2. Copy the application files to the folder where the application will be located.
3. In the `appsettings.json` file, fill in the `DefaultConnection` setting in the `ConnectionStrings` section according to the database connection information from the prerequisites section.

    For example: `"DefaultConnection": "Server=MyServer;Database=GitlabComments;User Id=user;Password=password;MultipleActiveResultSets=true"`
4. To use the client-side, place the contents of the folder from the corresponding [client-side application release](https://github.com/bodynar/MAS.GitlabComments.Client/releases) in the `\ClientApp` folder.
5. Start the application (depending on the deployment type: either IIS or dotnet).

#### Updating the Application
1. Stop the running application.
2. Back up the database and the application folder to a separate directory.
3. Completely delete the contents of the application folder except for the `ClientApp` folder.
4. Move the files from the new version of the application to the application folder.
5. In the `appsettings.json` file, transfer your customizations (including connection settings) from the backup folder (step 1) to the existing settings, without breaking the file format or the set of settings.
6. Execute all scripts from the `\SqlScripts` folder in the database. If errors occur, abort the installation process and contact the publisher (application author).
7. Start the application (depending on the deployment type: either IIS or dotnet).

If errors occur during the update process, restore the application state from the backup directory (step 1) and contact the publisher (application author).

### Configuration

The application supports configuration by editing the `appsettings.json` file.

## Development

For development, the following programs must be installed:
- dotnet
- docker

The database for development can be created either locally or via the `docker-compose.yml` file using the command:
> docker-compose up -d

This command will create two Docker containers:
1. A container with PostgreSQL (accessible at `localhost:5400`, user: `dev`, password: `123`, database name: `gc.dev`).
2. A container with MSSQL (accessible at `localhost:5401`, user: `sa`, password: `Pass_Word!12345678`).
    - After creating the Docker container, you need to manually create the database by executing a [T-SQL query](https://www.w3schools.com/sql/sql_create_db.asp).

The containers use Docker volumes, so they can be safely restarted.

### DBMS Support
Currently (version 1.5), the solution should work on both PostgreSQL and MSSQL.

## Related Links
* [Dotnet deploy with cli](https://learn.microsoft.com/en-us/dotnet/core/deploying/deploy-with-cli)
* [Dotnet run](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run)
* [Host ASP.NET Core on Windows with IIS](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-8.0)
* [Docker compose](https://docs.docker.com/compose/)