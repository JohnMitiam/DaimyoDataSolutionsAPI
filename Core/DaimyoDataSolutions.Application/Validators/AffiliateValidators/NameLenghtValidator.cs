using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.AffiliateValidators
{
    public class NameLenghtValidator : BaseValidator<Affiliate>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(Affiliate value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Name) || value.Name.Length <= 50;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Name exceeds 100 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Affiliate value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
