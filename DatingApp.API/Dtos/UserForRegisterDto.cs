using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
  // Dtos are used to map json objects from over the wire to objects to be used in the backend for processing
  public class UserForRegisterDto
  {
    [Required]
    public string Username { get; set; }
    [Required]
    [StringLength(8, MinimumLength = 4, ErrorMessage = "Must be between 4 and 8 chars.")]
    public string Password { get; set; }
    [Required]
    public string Gender { get; set; }
    [Required]
    public string KnownAs { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Country { get; set; }
    // created at is populated in the constructor here and is not sent with the req
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }

    public UserForRegisterDto()
    {
      Created = DateTime.Now;
      LastActive = DateTime.Now;
    }
  }
}