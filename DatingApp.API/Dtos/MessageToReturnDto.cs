using System;
/*
Note on AutoMapper magic with matching KnownAs and PhotoUrl:
AutoMapper is convention based.  It will automatically map properties that it can.  In this case we have the SenderId as a navigation property in the Message class, 
and it's smart enough to know that the Id part of SenderId is the ID and anything before that (Sender) must be the User object.  
It then matches properties that it can in the User class with the name of the property - in this case KnownAs.
*/

namespace DatingApp.API.Dtos
{
  public class MessageToReturnDto
  {
    public int Id { get; set; }
    public int SenderId { get; set; }
    // NOTE: AutoMapper magic will recognize if the SenderId relates to a User Id, the it will go and grab the KnownAs and PhotoUrl props
    // since we prepend them with the same name that comes befor `Id` above
    public string SenderKnownAs { get; set; } // automapper will actually get the KnownAs and PhotoUrl props from the User tied to the SenderId and populate these automatically (!@@#!!!)
    public string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public string RecipientKnownAs { get; set; }
    public string RecipientPhotoUrl { get; set; } // The photo url retrieval by AutoMapper needs to be configured manually in AutoMapper prfiles file (it does not work like the KnownAs did automatically)
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? DateRead { get; set; } // null if hasn't been read yet
    public DateTime MessageSent { get; set; }
  }
}