using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseSession _dbSession;

        public ProductRepository(DatabaseSession _dbSession)
        {
            this._dbSession = _dbSession;
        }

        public async Task<Products> CreateAsync(Products product)
        {
            var query = $@"sp_CreateProduct";

            var queryParams = new
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                CreatedBy = product.CreatedBy,
                DateCreated = product.DateCreated,
                IsDeleted = product.IsDeleted,
            };

            product.Id = await _dbSession.Connection.ExecuteScalarAsync<int>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            return product;
        }

        public async Task<bool> AddProductCategoriesAsync(int productId, IEnumerable<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any()) return true;

            await RemoveProductCategoriesAsync(productId);
            var uniqueIds = categoryIds.Distinct().ToList();

            var sql = "INSERT INTO ProductCategories (ProductsId, Id) VALUES (@ProductsId, @Id);";
            var parameters = uniqueIds.Select(id => new { ProductsId = productId, Id = id });

            await _dbSession.Connection.ExecuteAsync(sql, parameters, _dbSession.Transaction);
            return true;
        }

        public async Task<bool> RemoveProductCategoriesAsync(int productId)
        {
            await _dbSession.Connection.ExecuteAsync(
                "DELETE FROM ProductCategories WHERE ProductsId = @ProductsId;",
                new { ProductsId = productId }, _dbSession.Transaction);
            return true;
        }

        public async Task<(IEnumerable<Products> products, int recordCount)> GetAsync(ProductResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseQuery = "FROM Products WHERE IsDeleted = 0 ";
            var dataSql = "SELECT * " + baseQuery + queryParamBuilder.GetSearchSQLQuery() + queryParamBuilder.GetFilterSQLQuery() + queryParamBuilder.GetPaginationSQLQuery();
            var countSql = "SELECT COUNT(*) " + baseQuery + queryParamBuilder.GetSearchSQLQuery() + queryParamBuilder.GetFilterSQLQuery();

            var products = (await _dbSession.Connection.QueryAsync<Products>(dataSql, queryParamBuilder.Parameters)).ToList();
            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(countSql, queryParamBuilder.Parameters);

            if (products.Any())
            {
                var productIds = products.Select(p => p.Id).ToList();

                // ADD pc.ProductId to the SELECT list here:
                var sql = @"SELECT pc.ProductsId, c.Id, c.Name 
                    FROM ProductCategories pc 
                    INNER JOIN Category c ON pc.Id = c.Id 
                    WHERE pc.ProductsId IN @Ids";

                var details = await _dbSession.Connection.QueryAsync<dynamic>(sql, new { Ids = productIds }, _dbSession.Transaction);

                foreach (var product in products)
                {
                    product.ProductCategories = details
                        .Where(d => d.ProductsId == product.Id) // This will now work!
                        .Select(m => new ProductCategories
                        {
                            Id = m.Id,
                            Name = m.Name
                        }).ToList();
                }
            }
            return (products, count);
        }

        public async Task<(IEnumerable<Products> products, int count)> GetMyProductAsync(string userId)
        {
            // 1. Fetch the products filtered by CreatedBy
            var sql = @"
                SELECT * FROM Products 
                WHERE CreatedBy = @UserId 
                ORDER BY DateCreated DESC";

            var products = (await _dbSession.Connection.QueryAsync<Products>(
                sql,
                new { UserId = userId },
                _dbSession.Transaction)).ToList();

            int count = products.Count;

            // 2. Fetch the Categories for these specific products
            if (products.Any())
            {
                var productIds = products.Select(p => p.Id).ToList();

                var sqlCategories = @"
                SELECT pc.ProductsId, c.Id, c.Name 
                FROM ProductCategories pc
                INNER JOIN Category c ON pc.Id = c.Id
                WHERE pc.ProductsId IN @Ids";

                var details = await _dbSession.Connection.QueryAsync<dynamic>(
                    sqlCategories,
                    new { Ids = productIds },
                    _dbSession.Transaction
                );

                // 3. Map Categories back to the Product objects
                foreach (var product in products)
                {
                    product.ProductCategories = details
                        .Where(d => (int)d.ProductsId == product.Id)
                        .Select(m => new ProductCategories
                        {
                            Id = m.Id,
                            Name = m.Name
                        }).ToList();
                }
            }

            return (products, count);
        }

        public async Task<Products?> GetByIdAsync(int productId)
        {
            var product = await _dbSession.Connection.QueryFirstOrDefaultAsync<Products>(
                "sp_GetProductById", new { ID = productId }, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

            if (product != null)
            {
                var sql = @"SELECT c.Id, c.Name FROM ProductCategories pc 
                INNER JOIN Category c ON pc.Id = c.Id 
                WHERE pc.ProductsId = @ProductsId";

                product.ProductCategories = (await _dbSession.Connection.QueryAsync<ProductCategories>(
                    sql, new { ProductsId = productId }, _dbSession.Transaction)).ToList();
            }
            return product;
        }

        public async Task<bool> UpdateAsync(Products product)
        {
            var query = $@"sp_UpdateProduct";

            var queryParams = new
            { 
                ID = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                UpdatedBy = product.UpdatedBy,
                DateUpdated = product.DateUpdated
            };
            await _dbSession.Connection.ExecuteAsync(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return true;
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
    }
}