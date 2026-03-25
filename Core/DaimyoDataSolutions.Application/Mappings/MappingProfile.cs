using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.User;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<User, ViewUserDTO>();
        }
    }
}
