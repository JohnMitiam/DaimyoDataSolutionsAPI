using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.ProductValidators
{
    public class NameLengthValidator : BaseValidator<Product>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(Product value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Name) || value.Name.Length <= 100;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Name exceeds 100 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Product value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
