using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.DTOs.Product;
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

            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
            CreateMap<Product, ViewProductDTO>();

            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>();
        }
    }
}
