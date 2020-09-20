# Deploying to Production

### Setting up the API to Serve Static Files

- Set your SPA build files to be output to a folder in the root of the dotnet api project
- Set up Dotnet project to serve static files in Startup.cs
  - ```c#
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
