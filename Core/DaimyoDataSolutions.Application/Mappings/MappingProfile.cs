using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.DTOs.ProductCategories;

namespace DaimyoDataSolutions.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Affiliate, CreateAffiliateDTO>().ReverseMap();
            CreateMap<Affiliate, UpdateAffiliateDTO>().ReverseMap();
            CreateMap<Affiliate, ViewAffiliateDTO>();

            CreateMap<BaseProductDTO, Products>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());
            CreateMap<Products, CreateProductDTO>().ReverseMap();
            CreateMap<Products, UpdateProductDTO>().ReverseMap();
            CreateMap<Products, ViewProductDTO>();

            CreateMap<ProductCategories, ViewProductCategoriesDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Categories != null ? src.Categories.Name : null))
                .ForMember(dest => dest.CategoriesId, opt => opt.MapFrom(src => src.CategoryId));
            CreateMap<CreateProductCategoriesDTO, ProductCategories>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<UpdateProductCategoriesDTO, ProductCategories>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>();
        }
    }
}
