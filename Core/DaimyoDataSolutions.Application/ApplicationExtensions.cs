using Microsoft.Extensions.DependencyInjection;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.Mappings;
using DaimyoDataSolutions.Application.Services;
using DaimyoDataSolutions.Application.Validators.UserValidators;

namespace DaimyoDataSolutions.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IUserValidator, UserValidator>();

            return services;
        }
    }
}
