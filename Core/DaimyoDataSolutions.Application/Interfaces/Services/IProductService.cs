using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IServiceResult> GetAsync(ProductResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int productId);
        Task<IServiceResult> CreateAsync(CreateProductDTO product, string userId);
        Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO product, string userId);
        Task<IServiceResult> DeleteAsync(int product, string userId);
        Task<IServiceResult> GetMyProductsAsync(string userId);
    }
}
