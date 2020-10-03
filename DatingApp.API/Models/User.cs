using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
  public class User
  {
    public int Id { get; set; }

    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }


    public string Interests { get; set; }

    public string City { get; set; }
    public string Country { get; set; }

    // This prop is used for a relationship betqween Users and Photos, and the PhotoForDetailedDto is created to resolve not returning the User data with photos back to the client
    // NOTE: we implemented lazy loading with EF Core, so we need to make all navigation properties virtual!  We no longer need include statements in our repo and EF Core automagically knows what to include
    public virtual ICollection<Photo> Photos { get; set; }
    public virtual ICollection<Like> Likers { get; set; } // connection with Like entity which is a table that holds likers and likee info for likes, Like table is setup in the DataContext.cs file
    public virtual ICollection<Like> Likees { get; set; }  // because the like entity also has User props this sets up some kind of connection???
    public virtual ICollection<Message> MessagesSent { get; set; }
    public virtual ICollection<Message> MessagesReceived { get; set; }

  }
}