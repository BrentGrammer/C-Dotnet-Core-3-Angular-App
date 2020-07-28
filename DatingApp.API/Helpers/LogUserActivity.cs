using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Helpers
{
  /*
      This is an action filter class that updates the user's last activity everytime those methods in the users controller are hit.
      This prevents us from having to update the last active field manually in every one of the methods we want to update it.
      An attribute is added to the user controller to allow access and use this action filter there.
      Remember to add this as a service in the startup.cs class as a scoped service since you want to create a new instance of this action filter per request
        */
  public class LogUserActivity : IAsyncActionFilter
  {
    // the first arg is used to run operations when the action is being executed, the second arg is used to run something after the action is executed
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      // the result context is a context of after the execution and gives us access to things like the HttpContext
      var resultContext = await next();
      // get the userId from the token sent with the request by accessing the request context httpcontext:
      var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
      // get the repo off of the request services on the http context - the request services are the ones injected via dependency injection in startup.cs
      // get the user and update the last active field
      var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
      var user = await repo.GetUser(userId);
      user.LastActive = DateTime.Now;
      await repo.SaveAll();
    }
  }
}