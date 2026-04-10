using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Interfaces.Data
{
    public interface IAffiliateRepository : IBaseRepository<Affiliate>
    {
        Task<(IEnumerable<Affiliate> affiliate, int recordCount)> GetAsync(AffiliateResourceParameters resourceParameters);
    }
}
