using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.DTOs.ProductCategories;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IProductCategoriesService
    {
        Task<IServiceResult> GetAsync(ProductCategoriesResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int productId);
        Task<IServiceResult> CreateAsync(CreateProductCategoriesDTO product, string userId);
        Task<IServiceResult> UpdateAsync(int productId, UpdateProductCategoriesDTO product, string userId);
        Task<IServiceResult> DeleteAsync(int product, string userId);
    }
}
