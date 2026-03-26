using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.ProductValidators
{
    public class DescriptionLenghtValidator : BaseValidator<Product>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(Product value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Description) || value.Description.Length <= 50;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Description exceeds 200 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Product value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
