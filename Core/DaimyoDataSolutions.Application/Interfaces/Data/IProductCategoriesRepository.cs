using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IProductCategoriesRepository : IBaseRepository<ProductCategories>
    {
        Task<(IEnumerable<ProductCategories> productCategories, int recordCount)> GetAsync(ProductCategoriesResourceParameters resourceParameters);
        Task<bool> ExistsAsync(int productId, int categoryId);

        Task<bool> ExistsAsyncExcludingId(int productId, int categoryId, int excludeId);
    }
}
