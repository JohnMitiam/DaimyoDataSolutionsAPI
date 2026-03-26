using Dapper;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using System.Data;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseSession _dbSession;

        public ProductRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var query = $@"sp_CreateProduct";

            var queryParams = new
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                CreatedBy = product.CreatedBy,
                DateCreated = product.DateCreated
            };

            product.Id = await _dbSession.Connection.ExecuteScalarAsync<int>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return product;
        }

        public async Task<(IEnumerable<Product> products, int recordCount)> GetAsync(ProductResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseDataQuery = @"SELECT * FROM Products WHERE IsDeleted = 0 ";
            var baseCountQuery = @"SELECT COUNT(*) FROM Products WHERE IsDeleted = 0 ";

            var searchSQL = queryParamBuilder.GetSearchSQLQuery();
            var filterSQL = queryParamBuilder.GetFilterSQLQuery();
            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery();

            var finalDataQuery = baseDataQuery + searchSQL + filterSQL + paginationSQL;
            var finalCountQuery = baseCountQuery + searchSQL + filterSQL;

            var result = await _dbSession.Connection.QueryAsync<Product>(finalDataQuery, queryParamBuilder.Parameters);
            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (result, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            var query = $@"sp_GetProductById";

            var queryParams = new
            {
                ProductID = productId
            };

            var result = await _dbSession.Connection
                .QueryFirstOrDefaultAsync<Product>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var query = $@"sp_DeleteProduct";

            var queryParams = new
            {
                ProductID = productId
            };

            await _dbSession.Connection.ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var query = $@"sp_UpdateProduct";

            var queryParams = new
            {
                ProductID = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                UpdatedBy = product.UpdatedBy,
                DateUpdated = product.DateUpdated,
            };

            await _dbSession.Connection
                .ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return true;
        }
    }
}