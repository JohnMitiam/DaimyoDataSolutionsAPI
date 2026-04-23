using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.ProductValidators
{
    public class NameLengthValidator : BaseValidator<Products>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(Products value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Name) || value.Name.Length <= 100;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Name exceeds 100 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Products value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
