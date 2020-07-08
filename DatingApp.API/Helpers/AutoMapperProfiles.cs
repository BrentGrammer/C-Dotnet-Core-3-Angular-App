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
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForDetailedDto>();
        }
    }
}