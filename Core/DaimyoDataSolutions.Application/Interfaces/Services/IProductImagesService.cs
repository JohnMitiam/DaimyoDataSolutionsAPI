using DaimyoDataSolutions.Application.DTOs.ProductImages;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IProductImagesService
    {
        Task<IServiceResult> GetAsync(ProductImagesResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int productImageId);
        Task<IServiceResult> CreateAsync(CreateProductImagesDTO productImage, string userId);
        Task<IServiceResult> UpdateAsync(int productImageId, UpdateProductImagesDTO product, string userId);
        Task<IServiceResult> DeleteAsync(int productImage, string userId);
    }
}
