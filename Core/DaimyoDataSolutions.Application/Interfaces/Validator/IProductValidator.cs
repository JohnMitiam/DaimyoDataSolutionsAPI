using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Validator
{
    public interface IProductValidator : IValidate<Product>
    {
        //Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(User user);
    }
}
