using Microsoft.Extensions.DependencyInjection;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.Mappings;
using DaimyoDataSolutions.Application.Services;
using DaimyoDataSolutions.Application.Validators.AffiliateValidators;
using DaimyoDataSolutions.Application.Validators.ProductValidators;
using DaimyoDataSolutions.Application.Validators.CategoryValidators;

namespace DaimyoDataSolutions.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            services.AddTransient<IAffiliateService, AffiliateService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();

            services.AddTransient<IAffiliateValidator, AffiliateValidator>();
            services.AddTransient<IProductValidator, ProductValidator>();
            services.AddTransient<ICategoryValidator, CategoryValidator>();

            return services;
        }
    }
}
