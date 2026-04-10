using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.AffiliateValidators
{
    public class AffiliateValidator : IAffiliateValidator
    {
        private readonly IAffiliateRepository _affiliateRepository;

        public AffiliateValidator(IAffiliateRepository affiliateRepository)
        {
            _affiliateRepository = affiliateRepository;
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Affiliate value)
        {
            return await Task.FromResult(IsValid(value));
        }

        public (bool isSuccess, List<string>? errorMessages) IsValid(Affiliate value)
        {
            var nameLengthValidator = new NameLenghtValidator();
            var emailLengthValidator = new AffiliateEmailValidator();

            var affiliateValidator = nameLengthValidator
                .And(emailLengthValidator);

            var result = affiliateValidator.IsValid(value);
            return (result.isSuccess, result.errorMessages);
        }
    }
}
