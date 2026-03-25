using DaimyoDataSolutions.Application.DTOs.User;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IServiceResult> GetAsync(UserResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int uId);
        Task<IServiceResult> CreateAsync(CreateUserDTO user, string userId);
        Task<IServiceResult> UpdateAsync(int uId, UpdateUserDTO user, string userId);
        Task<IServiceResult> DeleteAsync(int uId, string userId);
    }
}
