namespace DatingApp.API.Dtos
{
    public class UserForLoginDto
    {
        // Note: No validation is needed here since the back end is going to process and check values against the database anyways
        public string Username { get; set; }
        public string Password { get; set; }
    }
}