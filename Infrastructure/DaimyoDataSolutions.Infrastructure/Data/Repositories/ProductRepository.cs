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

        public ProductRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<Products> CreateAsync(Products product)
        {
            var query = @"sp_CreateProduct";

            var queryParams = new
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                CreatedBy = product.CreatedBy,
                DateCreated = product.DateCreated,
                IsDeleted = product.IsDeleted
            };

            product.Id = await _dbSession.Connection.ExecuteScalarAsync<int>(
                query,
                queryParams,
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
            );

            return product;
        }

        public async Task<(IEnumerable<Products> products, int recordCount)> GetAsync(ProductResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseFromClause = @"
                FROM Products p 
                LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId AND pc.IsDeleted = 0
                LEFT JOIN Category c ON pc.CategoryId = c.Id 
                WHERE p.IsDeleted = 0 ";

            var dataSql = "SELECT p.*, pc.*, c.* " +
                          baseFromClause +
                          queryParamBuilder.GetSearchSQLQuery() +
                          queryParamBuilder.GetFilterSQLQuery();

            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery().Replace("ORDER BY Id", "ORDER BY p.Id");

            var finalDataQuery = dataSql + paginationSQL;
            var finalCountQuery = "SELECT COUNT(DISTINCT p.Id) " + baseFromClause + queryParamBuilder.GetSearchSQLQuery() + queryParamBuilder.GetFilterSQLQuery();

            var productDict = new Dictionary<int, Products>();

            await _dbSession.Connection.QueryAsync<Products, ProductCategories, Category, Products>(
                finalDataQuery,
                (product, pc, category) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var currentProduct))
                    {
                        currentProduct = product;
                        currentProduct.ProductCategories = new List<ProductCategories>();
                        productDict.Add(currentProduct.Id, currentProduct);
                    }

                    if (pc != null && pc.Id != 0)
                    {
                        pc.Category = category;
                        currentProduct.ProductCategories.Add(pc);
                    }
                    return currentProduct;
                },
                queryParamBuilder.Parameters,
                _dbSession.Transaction,
                splitOn: "Id,Id"
            );

            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (productDict.Values, totalCount);
        }

        public async Task<(IEnumerable<Products> products, int count)> GetMyProductAsync(string userId)
        {
            var baseFromClause = @"
                FROM Products p 
                LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId AND pc.IsDeleted = 0
                LEFT JOIN Category c ON pc.CategoryId = c.Id 
                WHERE p.CreatedBy = @UserId AND p.IsDeleted = 0 ";

            var sql = "SELECT p.*, pc.*, c.* " + baseFromClause + " ORDER BY p.DateCreated DESC";

            var productDict = new Dictionary<int, Products>();

            await _dbSession.Connection.QueryAsync<Products, ProductCategories, Category, Products>(
                sql,
                (product, pc, category) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var currentProduct))
                    {
                        currentProduct = product;
                        currentProduct.ProductCategories = new List<ProductCategories>();
                        productDict.Add(currentProduct.Id, currentProduct);
                    }

                    if (pc != null && pc.Id != 0)
                    {
                        pc.Category = category;
                        currentProduct.ProductCategories.Add(pc);
                    }
                    return currentProduct;
                },
                new { UserId = userId },
                _dbSession.Transaction,
                splitOn: "Id,Id"
            );

            return (productDict.Values, productDict.Count);
        }

        public async Task<Products?> GetByIdAsync(int productId)
        {
            var sql = "sp_GetProductById";
            var productDict = new Dictionary<int, Products>();

            var result = await _dbSession.Connection.QueryAsync<Products, ProductCategories, Category, Products>(
                sql,
                (product, pc, category) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var currentProduct))
                    {
                        currentProduct = product;
                        currentProduct.ProductCategories = new List<ProductCategories>();
                        productDict.Add(currentProduct.Id, currentProduct);
                    }

                    if (pc != null)
                    {
                        pc.Category = category;
                        pc.ProductId = currentProduct.Id;
                        currentProduct.ProductCategories.Add(pc);
                    }
                    return currentProduct;
                },
                new { p_ID = productId },
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "ProductCategoryId,CategoryId"
            );

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(Products product)
        {
            var query = @"sp_UpdateProduct";

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

        public async Task<bool> DeleteAsync(int id)
        {
            var query = @"sp_DeleteProduct";

            var rowsAffected = await _dbSession.Connection.ExecuteAsync(
                query,
                new { ProductID = id },
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Products product)
        {
            return await DeleteAsync(product.Id);
        }

        public async Task<bool> CategoryExistsAsync(int specificationId)
        {
            var sql = "SELECT COUNT(1) FROM Category WHERE Id = @Id AND IsDeleted = 0";

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(
                sql,
                new { Id = specificationId },
                _dbSession.Transaction
            );

            return count > 0;
        }
    }
}