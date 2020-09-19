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

      // first param is source(Map from) and second is the destination(Map to)
      // You are putting the values of the source into the matching properties of the destination(for instance taking the vals from a request dto and putting them into a model to insert into the database)

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
      // this case is mapping the dto which represents what is received from the client to the model we're going to save in the backend
      //the previous maps were taking the model and mapping it to dtos which we send to the client.
      CreateMap<UserForUpdateDto, User>();
      CreateMap<Photo, PhotoForReturnDto>();
      CreateMap<PhotoForCreationDto, Photo>();
      CreateMap<UserForRegisterDto, User>();
      //  ReverseMap() both allows you to map the request dto to the database model for storing in the database and also allows the other way so you can also map the model to a dto to return to the client
      // purpose for doing this is so you don't include passwords or sensitiv/unneeded data in the response after adding to the database
      // (see Messages Controller)
      CreateMap<MessageForCreationDto, Message>().ReverseMap();
      CreateMap<Message, MessageToReturnDto>()
        .ForMember(dest => dest.SenderPhotoUrl, opts => opts.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url)) // get the main photourl for each user in the message
        .ForMember(dest => dest.RecipientPhotoUrl, opts => opts.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url)); // get the main photourl for each user in the message
    }
  }
}