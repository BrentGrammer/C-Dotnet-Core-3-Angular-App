using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
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

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _repo.GetUsers();
      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
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
  }
}