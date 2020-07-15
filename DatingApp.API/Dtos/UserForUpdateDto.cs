namespace DatingApp.API.Dtos
{
  // this is the dto for the data coming from the client on the member edit form and will be mapped to a user object to store in the database
  public class UserForUpdateDto
  {
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
  }
}