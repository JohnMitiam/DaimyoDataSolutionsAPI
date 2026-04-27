using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IProductRepository : IBaseRepository<Products>
    {
        Task<(IEnumerable<Products> products, int recordCount)> GetAsync(ProductResourceParameters resourceParameters);
        Task<bool> AddProductCategoriesAsync(int productId, IEnumerable<int> categoryIds);
        Task<bool> RemoveProductCategoriesAsync(int productId);
        Task<(IEnumerable<Products> products, int count)> GetMyProductAsync(string userId);
    }
}
