using System;
using System.Collections.Generic;
using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
  public class UserForDetailedDto
  {
    public int Id { get; set; }

    public string Username { get; set; }

    public string Gender { get; set; }
    public int Age { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }


    public string Interests { get; set; }

    public string City { get; set; }
    public string Country { get; set; }
    //PhotoUrl is going to be populated by using the AutoMapper in AutoMApperProfiles.cs to grab the main photo url from the Photos on a User Model
    public string PhotoUrl { get; set; }
    //Note the type of the photo dto created so that the Navigation User prop on a Photo does not return the User data in each photo - necesary since AutoMapper does not know about EF relationships 
    public ICollection<PhotosForDetailedDto> Photos { get; set; }
  }
}