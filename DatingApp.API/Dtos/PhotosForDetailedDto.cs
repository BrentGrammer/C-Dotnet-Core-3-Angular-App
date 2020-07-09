using System;

namespace DatingApp.API.Dtos
{
  // This DTO is needed because there is a navigation property in the Photo/User models linking them, so the photos are being returned with User data
  // This happens because AutoMapper does not know that the User property on the photos is only for EF relationships
  // This is returned as the Photos property on the UserForDetailedDto instead of the Photo model
  public class PhotosForDetailedDto
  {
    public int Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsMain { get; set; }
  }
}