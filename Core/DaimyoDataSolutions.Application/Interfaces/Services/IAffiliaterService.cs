using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IAffiliateService
    {
        Task<IServiceResult> GetAsync(AffiliateResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int affiliateId);
        Task<IServiceResult> CreateAsync(CreateAffiliateDTO affiliate);
        Task<IServiceResult> UpdateAsync(int affiliateId, UpdateAffiliateDTO affiliate);
        Task<IServiceResult> DeleteAsync(int affiliateId);
    }
}
