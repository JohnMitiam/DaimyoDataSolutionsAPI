using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.CategoryValidators
{
    public class NameLengthValidator : BaseValidator<Category>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(Category value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Name) || value.Name.Length <= 50;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Name exceeds 50 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Category value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
