using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IProductRepository : IBaseRepository<Products>
    {
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Products> products, int recordCount)> GetAsync(ProductResourceParameters resourceParameters);
        Task<(IEnumerable<Products> products, int count)> GetMyProductAsync(string userId);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}
