using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IProductImagesRepository : IBaseRepository<ProductImages>
    {
        Task<(IEnumerable<ProductImages> productImages, int recordCount)> GetAsync(ProductImagesResourceParameters resourceParameters);
        Task<bool> PrimaryImageExistsAsync(int productId, int excludeImageId = 0);
    }
}
