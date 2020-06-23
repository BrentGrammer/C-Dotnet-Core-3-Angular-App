using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    // make the extension class static since you don't want to create any instances
    public static class Extensions
    {
        // preface the first arg with this modifier to indicate you are adding on to that class
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            // modify the headers to be able to add a error message and add CORS header
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

    }
}