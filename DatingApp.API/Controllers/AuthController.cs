using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMapper _mapper;
    public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
    {
      _mapper = mapper;
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

      // Map the source dto from the request to fill values in the User model destination for loading that into the database
      var userToCreate = _mapper.Map<User>(userForRegisterDto);

      var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

      // this is used as the user object to return to the client since we don't want o return password info etc. on the userToCreate which has all of that.
      var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

      // return a CreatedAt res which has the location header of where to find the resource and the resource itself included in the res.
      // the location is the route where the resource can be found (the api get route for it), and this is referenced by the Name prop added to the resource get route in the UsersController
      // the first arg is the route name, the second is route vals which are the controller name where the  route is and the params for the route (the id in this case), third arg is the resource mapped appropriately for sending to the client
      return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
    }

    // a separate dto user obj for login is created because only username and password will be needed - in the register there will be more info added and fields present
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

      // only return a 404 with no extra info for security purposes
      if (userFromRepo == null)
        return Unauthorized();

      // build a token to send to the client for future requests
      // can add user information to it so that can be used to authorize user instead of looking up username and info in the database on every req
      // this info is referred to as "claims"
      var claims = new[]
      {
               // NAmeIdentifier is closest to the idea of an Id to be stored, also cast it to a String since the JWT info is a string'
               // the name identifier is stored on the token as 'nameid' which is how you access it on the frontend.
               
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

      // we need to send the user's main photo url to the frontend for the navbar to display, so we reuse the userforlistdto which has what we want to send with the response
      // maybe should create a separate dto with just what we need, but this is done to save time
      var user = _mapper.Map<UserForListDto>(userFromRepo);

      // send this back to the client in an object containing a token property:
      return Ok(new
      {
        // The user's main photo is also sent with this response to avoid adding it to the token (which increases its size and it is sent many times to the backend)
        // the user's main photo will be stored in localstorage on the frontend and accessible to update there whenever the user changes their main photo 
        token = tokenHandler.WriteToken(token),
        user
      });

    }
  }
}