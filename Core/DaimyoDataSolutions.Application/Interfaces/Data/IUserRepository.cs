using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<(IEnumerable<User> users, int recordCount)> GetAsync(UserResourceParameters resourceParameters);
        //Task<(IEnumerable<User> users, int recordCount)> GetAllAsync();

        //Task<bool> IsInUseAsync(int id);
    }
}
