using Microsoft.Extensions.DependencyInjection;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.Mappings;
using DaimyoDataSolutions.Application.Services;
using DaimyoDataSolutions.Application.Validators.UserValidators;
using DaimyoDataSolutions.Application.Validators.ProductValidators;
using DaimyoDataSolutions.Application.Validators.CategoryValidators;

namespace DaimyoDataSolutions.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();

            services.AddTransient<IUserValidator, UserValidator>();
            services.AddTransient<IProductValidator, ProductValidator>();
            services.AddTransient<ICategoryValidator, CategoryValidator>();

            return services;
        }
    }
}
