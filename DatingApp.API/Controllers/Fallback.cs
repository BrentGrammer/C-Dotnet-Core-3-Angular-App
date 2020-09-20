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