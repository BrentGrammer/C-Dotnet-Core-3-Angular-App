namespace DatingApp.API.Helpers
{
  // These values are populated and mapped from the appsettings.json cloudinary values in the startup.cs class configuring the Cloundinary service
  public class CloudinarySettings
  {
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
  }
}