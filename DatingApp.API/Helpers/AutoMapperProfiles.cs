using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{

  // This is needed for AutoMapper to map from source to destination objects 
  public class AutoMapperProfiles : Profile
  {
    // create a constructor where you can create your mappings:
    //Note: convention based naming (props in dtos and models are the same) require no configuration - automapper will map them
    // for properties that don't match extra config is needed.
    public AutoMapperProfiles()
    {

      // first param is source and second is the destination
      //ForMember is used to populate a property a certain way on the Dto using access to the source model object
      // The PhotoUrl on the destination(UserForListDto) is populated from the Source(model) by getting users photos and finding the one with isMain is true:
      // The Age prop on the dto is populated by creating a extension method for DateTime type that can be used to calculate the age on the DoB
      CreateMap<User, UserForListDto>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
      CreateMap<User, UserForDetailedDto>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
      CreateMap<Photo, PhotosForDetailedDto>();
    }
  }
}