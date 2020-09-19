using System;
/*
  represents what is sent in a request from the client
*/
namespace DatingApp.API.Dtos
{
  public class MessageForCreationDto
  {
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public DateTime MessageSent { get; set; }
    public string Content { get; set; }
    public MessageForCreationDto()
    {
      MessageSent = DateTime.Now;
    }
  }
}