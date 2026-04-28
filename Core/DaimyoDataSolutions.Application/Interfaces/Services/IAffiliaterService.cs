using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Interfaces.Services
{
    public interface IAffiliateService
    {
        Task<IServiceResult> GetAsync(AffiliateResourceParameters resourceParameters);
        Task<IServiceResult> GetByIdAsync(int affiliateId);
        Task<IServiceResult> CreateAsync(CreateAffiliateDTO affiliate, string userId);
        Task<IServiceResult> UpdateAsync(int affiliateId, UpdateAffiliateDTO affiliate, string userId);
        Task<IServiceResult> DeleteAsync(int affiliateId, string userId);
    }
}
