using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Infrastructure.Data;
using DaimyoDataSolutions.Infrastructure.Data.Repositories;

namespace DaimyoDataSolutions.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DbConnection")
                    ?? throw new InvalidOperationException("Connection string 'DbConnection' not found."),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DbConnection"))));

            services.AddScoped<DatabaseSession>();

            services.AddTransient<IAffiliateRepository, AffiliateRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
