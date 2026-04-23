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

            CreateMap<ProductCategories, ViewProductCategoryDTO>();

            CreateMap<BaseProductDTO, Products>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());
            CreateMap<Products, CreateProductDTO>().ReverseMap();
            CreateMap<Products, UpdateProductDTO>().ReverseMap();
            CreateMap<Products, ViewProductDTO>();

            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>();
        }
    }
}
