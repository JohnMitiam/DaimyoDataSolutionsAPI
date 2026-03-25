using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Validator
{
    public interface IUserValidator : IValidate<User>
    {
        //Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(User user);
    }
}
