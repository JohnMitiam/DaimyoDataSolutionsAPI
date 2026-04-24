using Dapper;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.ResourceParameters;
using System.Data;
using DaimyoDataSolutions.Application.Interfaces.Data;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class AffiliateRepository : IAffiliateRepository
    {
        private readonly DatabaseSession _dbSession;

        public AffiliateRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<Affiliate> CreateAsync(Affiliate affiliate)
        {
            var query = $@"sp_CreateAffiliate";

            var queryParams = new
            {
                Name = affiliate.Name,
                Email = affiliate.Email,
                Phone = affiliate.Phone,
                Status = affiliate.Status,
                IsActive = affiliate.IsActive,
                CreatedBy = affiliate.CreatedBy,
                DateCreated = affiliate.DateCreated
            };

            affiliate.Id = await _dbSession.Connection
                .ExecuteScalarAsync<int>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return affiliate;
        }

        public async Task<(IEnumerable<Affiliate> affiliate, int recordCount)> GetAsync(AffiliateResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseDataQuery = @"SELECT * FROM Affiliate WHERE IsDeleted = 0 ";
            var baseCountQuery = @"SELECT COUNT(*) FROM Affiliate WHERE IsDeleted = 0 ";

            var searchSQL = queryParamBuilder.GetSearchSQLQuery();
            var filterSQL = queryParamBuilder.GetFilterSQLQuery();
            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery();

            var finalDataQuery = baseDataQuery + searchSQL + filterSQL + paginationSQL;
            var finalCountQuery = baseCountQuery + searchSQL + filterSQL;

            var result = await _dbSession.Connection.QueryAsync<Affiliate>(finalDataQuery, queryParamBuilder.Parameters);
            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (result, totalCount);
        }

        public async Task<Affiliate?> GetByIdAsync(int affiliateId)
        {
            var query = $@"sp_GetAffiliateById";

            var queryParams = new
            {
                ID = affiliateId
            };

            var result = await _dbSession.Connection
                .QueryFirstOrDefaultAsync<Affiliate>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAsync(int affiliateId)
        {
            var query = $@"sp_DeleteAffiliate";

            var queryParams = new
            {
                AffiliateID = affiliateId
            };

            await _dbSession.Connection.ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> UpdateAsync(Affiliate affiliate)
        {
            var query = $@"sp_UpdateAffiliate";

            var queryParams = new
            {
                ID = affiliate.Id,
                Name = affiliate.Name,
                Email = affiliate.Email,
                Phone = affiliate.Phone,
                Status = affiliate.Status,
                IsActive = affiliate.IsActive,
                UpdatedBy = affiliate.UpdatedBy,
                DateUpdated = affiliate.DateUpdated
            };

            await _dbSession.Connection
                .ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return true;
        }
    }
}