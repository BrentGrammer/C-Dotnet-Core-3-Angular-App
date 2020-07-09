using System;
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
      // The application error header is used on the front end to display an error
      response.Headers.Add("Application-Error", message);
      response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
      response.Headers.Add("Access-Control-Allow-Origin", "*");
    }

    // this extension method created to be used in the AutoMapperProfiles for populating the age on the destination dto
    public static int CalculateAge(this DateTime theDateTime)
    {
      var age = DateTime.Today.Year - theDateTime.Year;
      // Check if the birthday has occurred this year or not, and if not then take off one year.
      if (theDateTime.AddYears(age) > DateTime.Today)
      {
        age--;
      }

      return age;
    }

  }
}