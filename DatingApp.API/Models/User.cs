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
    public ICollection<Photo> Photos { get; set; }
    public ICollection<Like> Likers { get; set; } // connection with Like entity which is a table that holds likers and likee info for likes, Like table is setup in the DataContext.cs file
    public ICollection<Like> Likees { get; set; }

  }

}