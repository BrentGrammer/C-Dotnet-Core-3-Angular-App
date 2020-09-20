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

    // {id} is coming from the query string and is added on to the end of the root 
    // ex: api/users/{userId}/messages/{id}"
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

    // This get does not conflict with the above because there is no id url query string we take in here - so it qualifies as a non-colliding route
    // the userId is coming from the route itself defined on line 21 /users/{userId}/messages
    // use [FromQuery] to tell Dotnet that the pagination params are coming from the query string
    [HttpGet]
    public async Task<IActionResult> GetMessagesForUser(int userId,
      [FromQuery] MessageParams messageParams)
    {
      // make sure user token matches user id passed in
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();
      // populate the user id prop on messageParams using the userId here
      messageParams.UserId = userId;

      var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

      var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

      // Return pagination details to response header from the PagedList result returned from repo
      Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

      return Ok(messages);
    }

    // Get Conversation between users - append 'thread' before the query placeholder since otherwise it would conflict with the {id} in GetMessage
    [HttpGet("thread/{recipientId}")]
    public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
    {
      // make sure user token matches user id passed in
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);
      // map the messages returned from Repo to the dto to return
      var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

      return Ok(messageThread);
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
        // "GetMessage" is the Name assigned to the method above
        return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);

      throw new Exception("Creating the message failed on save");
    }
  }
}