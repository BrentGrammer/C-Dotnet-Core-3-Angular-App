using System;

namespace DatingApp.API.Models
{
  public class Photo
  {
    public int Id { get; set; }

    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsMain { get; set; }
    // Public Id id returned from Cloudinary for photos and we save this in the database
    public string PublicId { get; set; }
    //create EF convention based relationship with the User (user has photos) by adding the model and id props
    //this also sets EF to do cascading delete so Photos related to the user are deleted when the user is deleted  
    public virtual User User { get; set; } // nav prop made virtual due to implementing EF Core lazy loading

    public int UserId { get; set; }
  }
}