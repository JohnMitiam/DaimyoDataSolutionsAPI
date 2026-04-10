using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Validator
{
    public interface ICategoryValidator : IValidate<Category>
    {
        //Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(User affiliate);
    }
}
