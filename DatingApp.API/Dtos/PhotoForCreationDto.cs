using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Dtos
{
  public class PhotoForCreationDto
  {
    public string Url { get; set; }
    // IFormFile is the type for a file sent in an http request - this will be the image file
    public IFormFile File { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    // Public id is returned from Cloudinary for the photo
    public string PublicId { get; set; }

    //create a constructor to populate the date added with now:
    public PhotoForCreationDto()
    {
      DateAdded = DateTime.Now;
    }
  }
}