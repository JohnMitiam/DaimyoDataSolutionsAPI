using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<(IEnumerable<Category> categories, int recordCount)> GetAsync(CategoryResourceParameters resourceParameters);
    }
}
