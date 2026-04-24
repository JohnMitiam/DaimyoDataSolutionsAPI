using Dapper;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using System.Data;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseSession _dbSession;

        public CategoryRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            var query = $@"sp_CreateCategory";

            var queryParams = new
            {
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                CreatedBy = category.CreatedBy,
                DateCreated = category.DateCreated,
                IsDeleted = category.IsDeleted
            };

            category.Id = await _dbSession.Connection.ExecuteScalarAsync<int>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return category;
        }

        public async Task<(IEnumerable<Category> categories, int recordCount)> GetAsync(CategoryResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseDataQuery = @"SELECT * FROM Category WHERE IsDeleted = 0 ";
            var baseCountQuery = @"SELECT COUNT(*) FROM Category WHERE IsDeleted = 0 ";

            var searchSQL = queryParamBuilder.GetSearchSQLQuery();
            var filterSQL = queryParamBuilder.GetFilterSQLQuery();
            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery();

            var finalDataQuery = baseDataQuery + searchSQL + filterSQL + paginationSQL;
            var finalCountQuery = baseCountQuery + searchSQL + filterSQL;

            var result = await _dbSession.Connection.QueryAsync<Category>(finalDataQuery, queryParamBuilder.Parameters);
            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (result, totalCount);
        }

        public async Task<Category?> GetByIdAsync(int categoryId)
        {
            var query = $@"sp_GetCategoryById";

            var queryParams = new
            {
                ID = categoryId
            };

            var result = await _dbSession.Connection
                .QueryFirstOrDefaultAsync<Category>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            var query = $@"sp_DeleteCategory";

            var queryParams = new
            {
                CategoryID = categoryId
            };

            await _dbSession.Connection.ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var query = $@"sp_UpdateCategory";

            var queryParams = new
            {
                ID = category.Id,
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                UpdatedBy = category.UpdatedBy,
                DateUpdated = category.DateUpdated,
            };

            await _dbSession.Connection
                .ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return true;
        }
    }
}