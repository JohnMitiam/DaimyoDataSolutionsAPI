using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IServiceResult> GetAsync(ProductResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int ProductId);
        Task<IServiceResult> CreateAsync(CreateProductDTO product);
        Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO product);
        Task<IServiceResult> DeleteAsync(int product);
    }
}
