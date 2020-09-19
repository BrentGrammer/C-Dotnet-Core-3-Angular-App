using System;

namespace DatingApp.API.Models
{
  /**
  Relationship - users send many messages to many users and recieve messages from many users
  This is added to the User class as a collection

  Remember to add this new model to the DataContext as a DbSet and configure it with a builder so EF
  is aware of the relationship
  */
  public class Message
  {
    public int Id { get; set; }
    public int SenderId { get; set; }
    public User Sender { get; set; } // Link to the user model?
    public int RecipientId { get; set; }
    public User Recipient { get; set; } // include link to the User Model to be able to use user info such as their main photo url
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? DateRead { get; set; } // null if hasn't been read yet
    public DateTime MessageSent { get; set; }
    // keep the message in database if only one of the members deleted their message, only delete the message if both of the members deleted it
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
  }
}