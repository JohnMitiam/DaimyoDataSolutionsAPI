using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Infrastructure.Data;
using DaimyoDataSolutions.Infrastructure.Data.Repositories;

namespace DaimyoDataSolutions.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<DatabaseSession>();

            // Stored Procedure Repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
