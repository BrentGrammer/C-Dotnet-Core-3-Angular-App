using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        // Note: because [ApiController] is being used on the class, the parameters (userForRegisterDto) will be automatically infered to be from a post body (otherewise you would need to specify [FromBody])
        // Remember to add the route name in the parens for nested routes - api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // validate request

            // make names lowercase to prevent duplicates with different casing
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("name already exists.");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        // a separate dto user obj for login is created because only username and password will be needed - in the register there will be more info added and fields present
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            throw new Exception("Eror");
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            // only return a 404 with no extra info for security purposes
            if (userFromRepo == null)
                return Unauthorized();

            // build a token to send to the client for future requests
            // can add user information to it so that can be used to authorize user instead of looking up username and info in the database on every req
            // this info is referred to as "claims"
            var claims = new[]
            {
               // NAmeIdentifier is closest to the idea of an Id to be stored, also cast it to a String since the JWT info is a string
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // assign a key used to sign the token - this is not human readable and is hashed:
            // key is stored in the appsettings file since it is used in other places in the app - 
            // the server needs to sign the token to use to validate it using the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // next create signing credentials:
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // now create a descriptor - this will contain claims, signing credentials and expiry date for the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // next create a token handler to be used and pass in the token descriptor
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // send this back to the client in an object containing a token property:
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
}