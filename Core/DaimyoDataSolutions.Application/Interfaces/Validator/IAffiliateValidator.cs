using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Validator
{
    public interface IAffiliateValidator : IValidate<Affiliate>
    {
        //Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(Affiliate affiliate);
    }
}
