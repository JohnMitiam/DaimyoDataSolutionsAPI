using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.CategoryValidators
{
    public class DescriptionLenghtValidator : BaseValidator<Category>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(Category value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Description) || value.Description.Length <= 100;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Description exceeds 100 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Category value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
