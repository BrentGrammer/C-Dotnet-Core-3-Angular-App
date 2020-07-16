using System;

namespace DatingApp.API.Dtos
{
  // This dto is created to use as the photo object to return from the GetPhoto route (instead of for the photo for member detail)
  // The only difference is the public id prop - this is done for clarity in the app.
  // remember to create a mapper for new dtos in the AutoMapper profiles file
  public class PhotoForReturnDto
  {
    public int Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }

  }
}