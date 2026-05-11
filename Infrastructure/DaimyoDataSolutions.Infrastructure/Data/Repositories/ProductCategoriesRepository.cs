using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        private readonly DatabaseSession _dbSession;

        public ProductCategoriesRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<ProductCategories> CreateAsync(ProductCategories productCategories)
        {
            var query = @"sp_CreateProductCategories";

            var queryparams = new
            {
                ProductId = productCategories.ProductId,
                CategoryId = productCategories.CategoryId,
                CreatedBy = productCategories.CreatedBy,
                DateCreated = productCategories.DateCreated,
                IsDeleted = productCategories.IsDeleted
            };

            productCategories.Id = await _dbSession.Connection.ExecuteScalarAsync<int>
                (
                query, queryparams, _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
                );

            return productCategories;
        }

        public async Task<bool> UpdateAsync(ProductCategories productCategories)
        {
            var query = @"sp_UpdateProductCategories";

            var queryParams = new
            {
                ID = productCategories.Id,
                ProductId = productCategories.ProductId,
                CategoryId = productCategories.CategoryId,
                UpdatedBy = productCategories.UpdatedBy,
                DateUpdated = productCategories.DateUpdated
            };

            await _dbSession.Connection
                .ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return true;
        }

        public async Task<bool> DeleteAsync(ProductCategories productCategories)
        {
            var query = $@"sp_DeleteProductCategories";

            var queryParams = new
            {
                ProductCategoryID = productCategories.Id
            };

            await _dbSession.Connection.ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<ProductCategories?> GetByIdAsync(int productCategoriesId)
        {
            var query = $@"sp_GetProductCategoriesById";

            var queryParams = new
            {
                ID = productCategoriesId
            };

            var result = await _dbSession.Connection
                .QueryFirstOrDefaultAsync<ProductCategories>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<(IEnumerable<ProductCategories> productCategories, int recordCount)> GetAsync(
            ProductCategoriesResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseDataQuery = @"SELECT * FROM ProductCategories WHERE IsDeleted = 0 ";
            var baseCountQuery = @"SELECT COUNT(*) FROM ProductCategories WHERE IsDeleted = 0 ";

            var searchSQL = queryParamBuilder.GetSearchSQLQuery();
            var filterSQL = queryParamBuilder.GetFilterSQLQuery();
            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery();

            var finalDataQuery = baseDataQuery + searchSQL + filterSQL + paginationSQL;
            var finalCountQuery = baseCountQuery + searchSQL + filterSQL;

            var result = await _dbSession.Connection.QueryAsync<ProductCategories>(finalDataQuery, queryParamBuilder.Parameters);
            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (result, totalCount);
        }

        public async Task<bool> ExistsAsync(int productId, int categoryId)
        {
            var query = @"SELECT COUNT(1) FROM ProductCategories
                            WHERE ProductId = @ProductId
                            AND CategoryId = @CategoryId
                            AND IsDeleted = 0";

            var queryParams = new
            {
                ProductId = productId,
                CategoryId = categoryId
            };

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>
                (
                    query, queryParams, _dbSession.Transaction
                );

            return count > 0;
        }

        public async Task<bool> ExistsAsyncExcludingId(int productId, int categoryId, int excludeId)
        {
            var query = @"SELECT COUNT(1) FROM ProductCategories
                            WHERE ProductId = @ProductId
                            AND CategoryId = @CategoryId
                            AND Id != @Excluded
                            AND IsDeleted = 0";

            var queryParams = new
            {
                ProductId = productId,
                CategoryId = categoryId,
                ExcludeId = excludeId
            };

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>
                (
                    query, queryParams, _dbSession.Transaction
                );

            return count > 0;
        }
    }
}
