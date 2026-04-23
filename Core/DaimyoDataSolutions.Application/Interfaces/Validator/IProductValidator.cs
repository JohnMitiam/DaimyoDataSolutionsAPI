using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Validator
{
    public interface IProductValidator : IValidate<Products>
    {
        //Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(User affiliate);
    }
}
