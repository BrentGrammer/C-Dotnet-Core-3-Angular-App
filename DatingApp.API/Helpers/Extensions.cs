using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

    //this adds pagination info to the header of the response for the client to use to display and how to request the next batch of items using the Pagination Header class
    public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
      // the headers are added in title case by default, but you can format them to be camel case since that is more idiomatic in JavaScipt with the angular front end:
      var camelCaseFormatter = new JsonSerializerSettings();
      camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
      response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
      // expose the header to avoid getting CORS errors:
      response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
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