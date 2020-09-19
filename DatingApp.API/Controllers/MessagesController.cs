using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/*
    Path is api/users/{loggedinuserId}/messages
*/

namespace DatingApp.API.Controllers
{
  [ServiceFilter(typeof(LogUserActivity))] // update user last active date
  [Authorize]
  [Route("api/users/{userId}/[controller]")]
  [ApiController]
  public class MessagesController : ControllerBase
  {
    private readonly IDatingRepository _repo;
    private readonly IMapper _mapper;
    public MessagesController(IDatingRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;
    }

    [HttpGet("{id}", Name = "GetMessage")]
    public async Task<IActionResult> GetMessage(int userId, int id)
    {
      // make sure user token matches user id passed in
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var messageFromRepo = await _repo.GetMessage(id);

      if (messageFromRepo == null)
        return NotFound();

      return Ok(messageFromRepo);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
    {
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();


      messageForCreationDto.SenderId = userId;

      var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

      if (recipient == null)
        return BadRequest("Could not find user");

      // map request dto into our message entity
      var message = _mapper.Map<Message>(messageForCreationDto);

      _repo.Add(message); // not async - not querying or doing anything with the db at this time

      // you need to map the message model back to a dto to return to the client so you don't return password info, etc. in th response
      // you might want to create a separate response dto for this, but the one we have suffices for this example
      var messageToReturn = _mapper.Map<MessageForCreationDto>(message);

      if (await _repo.SaveAll())
        return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);

      throw new Exception("Creating the message failed on save");
    }
  }
}