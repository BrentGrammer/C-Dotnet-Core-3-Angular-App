# Deploying to Production

Note: see README.md for more Angular specific steps. This is aimed at being a more general guide for working with SPA client projects

### Setting up the API to Serve Static Files

- Set your SPA build files to be output to a folder in the root of the dotnet api project
- Set up Dotnet project to serve static files in Startup.cs
  - ````c#
          /*
            Setup for serving static files (i.e. from your SPA client)
            -For this to work, you need to have your SPA build script output the build files to a folder in the root
            of the dotnet project - in this case wwwroot folder in DatingApp.API
          */
          // look for default asset files like index.html etc., if it finds the file, it serves it
          app.UseDefaultFiles();
          // add ability for web server to use static files
          app.UseStaticFiles();
        ```
    Note that Angular 8+ uses differential loading when you use `ng build`
    ````

### Setup .NET project to defer and use SPA routing

- .NET does not know how to handle the SPA routes in your client and will break
- Create a Fallback controller that inherits from Controller for view support

  - ```c#
      using System.IO;
      using Microsoft.AspNetCore.Mvc;

      namespace DatingApp.API.Controllers
      {
        /*
            The purpose of this controller is to allow for .NET to defer to Angular Routing when a angular route is hit
            in the SPA client.  Otherwise, .NET does not know what to serve.

            Only inherit from `Controller` and not `ControllerBase` as in our other controllers, because we need View support.
        */
        public class Fallback : Controller
        {
          public IActionResult Index()
          {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/html");
          }
        }
      }
    ```

- In `Startup.cs` in the `Configure()` block, use the `endpoints.MapFallbackToController()` method to tell .NET that any routes it does not recognize should use the Fallback controller created above
  - ```c#
        app.UseEndpoints(endpoints =>
            {
              endpoints.MapControllers();
              // tell .NET to use the Fallback controller (which serves index.html) for any routes it does not recognize
              // this is done to defer handling of Angular SPA routing to the client on angular routes
              // the first arg is the name of the method in the controller to use and the second is the name of the controller
              endpoints.MapFallbackToController("Index", "Fallback");
            });
    ```

### Setup Database Provider for Production

[See list of Providers that Entity Framework supports Here](https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)

- For SQLServer, install the Nuget package `Microsoft.EntityFrameworkCore.SqlServer`
  - Select a version that is closest to or matches your version of .NET Core/EF Core
- Add the connection string for Sqlite database to your `appsettings.Development.json` file:

```json
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=datingapp.db"
  },
```

- Add a Connection string for your prod database in your `app.settings.json` file

  - If supporting multiple databases, you'll need to switch the connection string value to conform to the appropriate format
  - For SQLServer, the string format is similar to:

  ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost; Database=datingapp; User Id=appuser; Password=password"
    },
  ```

  ### Creating a User for SQLServer

- In [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15) try creating a SQL User account (give it full permissions just to make sure it can create a DB on the server) and use the following type of connection string:
  - `Server=myServerAddress;Database=myDataBase;User Id=myUsername; Password=myPassword;`
  - Also make sure the SQL Server is actually running and you have something listening on 1433 here as well.

# Azure Devops

- (https://dev.azure.com/)

## Setup

- Go to (https://visualstudio.microsoft.com/) and click the Get Started For Free under Microsoft Azure
- (email is alternate gmail)
