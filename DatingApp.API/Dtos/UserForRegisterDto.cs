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
    }
}