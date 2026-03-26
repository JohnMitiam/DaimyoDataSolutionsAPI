using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<(IEnumerable<Product> products, int recordCount)> GetAsync(ProductResourceParameters resourceParameters);
    }
}
