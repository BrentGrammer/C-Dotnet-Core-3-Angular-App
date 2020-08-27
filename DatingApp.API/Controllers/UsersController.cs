using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
  //Service filter is used to use the action filter to update the user last active field for methods in this controller
  // any time any of the methods in this controller gets called, the action filter will run updating the last active prop n the user.
  [ServiceFilter(typeof(LogUserActivity))]
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IDatingRepository _repo;
    private readonly IMapper _mapper;
    public UsersController(IDatingRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;

    }

    // Note - [FromQuery] is used to tell Dotnet that theuser params are coming from the query string.  If you leave this out then you'll get an empty body error if no query params are set in the url
    //since dotnet does not know how to handle the request with empty query strings without being told the params are coming from that.
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams) //userParams is coming from the query string and mapped by dotnet
    {
      // return results based on gender - User is coming from the apicontroller which allows you to access credentials for the session user
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var userFromRepo = await _repo.GetUser(currentUserId);
      // if this is missing it causes a null reference error
      userParams.UserId = currentUserId;

      if (string.IsNullOrEmpty(userParams.Gender))
      {
        userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
      }

      var users = await _repo.GetUsers(userParams); // not converting this to a list enables you to use it as IQueryable and pass into the repo GetUsers method as such.
      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      // use the extension method on the Response available in thi controller context to send back pagination info
      // the pagination info is on the users result since the call to the repo now returns a PagedList and is put on the headers of the response using the extension method AddPagination
      Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
      return Ok(usersToReturn);
    }
    // the NAme prop is added to be able to reference the route as a location of the resource sent in the location header of a CreatedAt response (i.e. in the AuthController for RegisterUser)
    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id);
      // use automapper to return limited info on user (i.e. not password data) using AutoMapper package
      // First type passed in is the destination Dto class and the source model is passed into the parameter of .Map()
      // A Profiles class created in Helpers folder is needed by Automapper to map the source to the destination
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);
      return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
    {
      // the claim types on a user which contain the authorized user's id is stored as a string and it needs to be parsed to an into for comparison to the id in the url param
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

      var userFromRepo = await _repo.GetUser(id);

      // map the user dto coming in from the client to the user from the database to update those fields
      _mapper.Map(userForUpdateDto, userFromRepo);

      if (await _repo.SaveAll()) return NoContent();

      throw new Exception($"Updating user {id} failed on saving.");
    }

    [HttpPost("{id}/like/{recipientId}")]
    public async Task<IActionResult> LikeUser(int id, int recipientId)
    {
      // prevent others from saying another user and like someone else if they are not the user sending the request.
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

      // check if like already exists:
      var like = await _repo.GetLike(id, recipientId);

      if (like != null)
        return BadRequest("You already like this user.");
      // check if user being like exists (??)
      if (await _repo.GetUser(recipientId) == null)
        return NotFound();

      // at this point the like is null so you can populate it with the data for the like and get it ready to add to the database
      like = new Like
      {
        LikerId = id,
        LikeeId = recipientId
      };

      // NOTE: This is not async - you are not adding it to the database here, just saving it to memory at this stage (using the context)
      _repo.Add<Like>(like);

      // now you make the asynchronous call to save changes to the database
      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("Failed to like user.");

    }
  }
}