using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IServiceResult> GetAsync(CategoryResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int categoryId);
        Task<IServiceResult> CreateAsync(CreateCategoryDTO category);
        Task<IServiceResult> UpdateAsync(int categoryId, UpdateCategoryDTO category);
        Task<IServiceResult> DeleteAsync(int category);
    }
}
