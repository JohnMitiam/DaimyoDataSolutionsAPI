using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.DTOs.ProductCategories;
using DaimyoDataSolutions.Application.DTOs.ProductImages;

namespace DaimyoDataSolutions.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Affiliate ---
            CreateMap<Affiliate, CreateAffiliateDTO>().ReverseMap();
            CreateMap<Affiliate, UpdateAffiliateDTO>().ReverseMap();
            CreateMap<Affiliate, ViewAffiliateDTO>();

            // --- Category ---
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>();

            // --- Product Categories ---
            CreateMap<ProductCategories, ViewProductCategoriesDTO>()
                .ForMember(dest => dest.CategoryName,opt =>
                    opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CategoryId, opt =>
                    opt.MapFrom(src => src.CategoryId));

            CreateMap<CreateProductCategoriesDTO, ProductCategories>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<UpdateProductCategoriesDTO, ProductCategories>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // --- Product Images ---
            CreateMap<ProductImages, ViewProductImagesDTO>()
                .ForMember(dest => dest.ImageData, opt =>
                    opt.MapFrom(src => src.ImageData))
                .ForMember(dest => dest.MimeType, opt =>
                    opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.IsPrimary, opt =>
                    opt.MapFrom(src => src.IsPrimary));

            CreateMap<CreateProductImagesDTO, ProductImages>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<UpdateProductImagesDTO, ProductImages>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // --- Product ---
            CreateMap<ViewProductDTO, Products>().ReverseMap();

            CreateMap<CreateProductDTO, Products>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore())
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore());

            CreateMap<UpdateProductDTO, Products>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore())
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore());
        }
    }
}
