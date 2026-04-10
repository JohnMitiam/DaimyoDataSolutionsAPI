using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Affiliate, CreateAffiliateDTO>().ReverseMap();
            CreateMap<Affiliate, UpdateAffiliateDTO>().ReverseMap();
            CreateMap<Affiliate, ViewAffiliateDTO>();

            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
            CreateMap<Product, ViewProductDTO>();

            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>();
        }
    }
}
