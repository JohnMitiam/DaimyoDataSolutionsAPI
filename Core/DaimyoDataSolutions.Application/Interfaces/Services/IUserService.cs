using DaimyoDataSolutions.Application.DTOs.User;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IServiceResult> GetAsync(UserResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int userId);
        Task<IServiceResult> CreateAsync(CreateUserDTO user);
        Task<IServiceResult> UpdateAsync(int userId, UpdateUserDTO user);
        Task<IServiceResult> DeleteAsync(int userId);
    }
}
