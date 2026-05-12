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

            // 1. Updated Clause to include ProductImages join
            var baseFromClause = @"
                                FROM Products p 
                                LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId AND pc.IsDeleted = 0
                                LEFT JOIN Category c ON pc.CategoryId = c.Id 
                                LEFT JOIN ProductImages pi ON p.Id = pi.ProductId AND pi.IsDeleted = 0
                                WHERE p.IsDeleted = 0";

            // 2. Added pi.* to the selection
            var dataSql = "SELECT p.*, pc.*, c.*, pi.* " +
                          baseFromClause +
                          queryParamBuilder.GetSearchSQLQuery() +
                          queryParamBuilder.GetFilterSQLQuery();

            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery().Replace("ORDER BY Id", "ORDER BY p.Id");
            var finalDataQuery = dataSql + paginationSQL;

            // IMPORTANT: Count must use DISTINCT p.Id because joins multiply row counts
            var finalCountQuery = "SELECT COUNT(DISTINCT p.Id) " +
                                 baseFromClause +
                                 queryParamBuilder.GetSearchSQLQuery() +
                                 queryParamBuilder.GetFilterSQLQuery();

            var productDict = new Dictionary<int, Products>();

            await _dbSession.Connection.QueryAsync<Products, ProductCategories, Category, ProductImages, Products>(
                finalDataQuery,
                (product, pc, category, pi) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var currentProduct))
                    {
                        currentProduct = product;
                        currentProduct.ProductCategories = new List<ProductCategories>();
                        currentProduct.ProductImages = new List<ProductImages>();
                        productDict.Add(currentProduct.Id, currentProduct);
                    }

                    if (pc != null && pc.Id != 0 && !currentProduct.ProductCategories.Any(x => x.Id == pc.Id))
                    {
                        pc.Category = category;
                        currentProduct.ProductCategories.Add(pc);
                    }

                    if (pi != null && pi.Id != 0 && !currentProduct.ProductImages.Any(x => x.Id == pi.Id))
                    {
                        currentProduct.ProductImages.Add(pi);
                    }

                    return currentProduct;
                },
                queryParamBuilder.Parameters,
                _dbSession.Transaction,
                splitOn: "Id,Id,Id"
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
                                LEFT JOIN ProductImages pi ON p.Id = pi.ProductId AND pi.IsDeleted = 0
                                WHERE p.CreatedBy = @UserId AND p.IsDeleted = 0 ";

            var sql = "SELECT p.*, pc.*, c.*, pi.* " + baseFromClause + " ORDER BY p.DateCreated DESC";
            var productDict = new Dictionary<int, Products>();

            await _dbSession.Connection.QueryAsync<Products, ProductCategories, Category, ProductImages, Products>(
                sql,
                (product, pc, category, pi) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var currentProduct))
                    {
                        currentProduct = product;
                        currentProduct.ProductCategories = new List<ProductCategories>();
                        currentProduct.ProductImages = new List<ProductImages>();
                        productDict.Add(currentProduct.Id, currentProduct);
                    }

                    if (pc != null && pc.Id != 0 && !currentProduct.ProductCategories.Any(x => x.Id == pc.Id))
                    {
                        pc.Category = category;
                        currentProduct.ProductCategories.Add(pc);
                    }

                    if (pi != null && pi.Id != 0 && !currentProduct.ProductImages.Any(x => x.Id == pi.Id))
                    {
                        currentProduct.ProductImages.Add(pi);
                    }

                    return currentProduct;
                },
                new { UserId = userId },
                _dbSession.Transaction,
                splitOn: "Id,Id,Id"
            );

            return (productDict.Values, productDict.Count);
        }

        public async Task<Products?> GetByIdAsync(int productId)
        {
            const string sql = "sp_GetProductById";
            var productDict = new Dictionary<int, Products>();

            await _dbSession.Connection.QueryAsync<Products, ProductCategories, Category, ProductImages, Products>(
                sql,
                (product, pc, category, pi) =>
                {
                    if (!productDict.TryGetValue(product.Id, out var currentProduct))
                    {
                        currentProduct = product;
                        currentProduct.ProductCategories = new List<ProductCategories>();
                        currentProduct.ProductImages = new List<ProductImages>();
                        productDict.Add(currentProduct.Id, currentProduct);
                    }
                    if (pc != null && pc.Id != 0)
                    {
                        if (!currentProduct.ProductCategories.Any(x => x.Id == pc.Id))
                        {
                            pc.Category = category;
                            pc.ProductId = currentProduct.Id;
                            currentProduct.ProductCategories.Add(pc);
                        }
                    }
                    if (pi != null && pi.Id != 0)
                    {
                        if (!currentProduct.ProductImages.Any(x => x.Id == pi.Id))
                        {
                            currentProduct.ProductImages.Add(pi);
                        }
                    }

                    return currentProduct;
                },
                new { p_ID = productId },
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "ProductCategoryId,CategoryId,ProductImageId"
            );

            return productDict.Values.FirstOrDefault();
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

        public async Task<IEnumerable<ProductCategories>> GetCategoriesByProductIdAsync(int productId)
        {
            const string sql = "SELECT * FROM ProductCategories WHERE ProductId = @ProductId AND IsDeleted = 0";
            return await _dbSession.Connection.QueryAsync<ProductCategories>(sql, new { ProductId = productId }, _dbSession.Transaction);
        }

        public async Task<IEnumerable<ProductImages>> GetImagesByProductIdAsync(int productId)
        {
            const string sql = "SELECT * FROM ProductImages WHERE ProductId = @ProductId AND IsDeleted = 0";
            return await _dbSession.Connection.QueryAsync<ProductImages>(sql, new { ProductId = productId }, _dbSession.Transaction);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            var sql = "SELECT COUNT(1) FROM Category WHERE Id = @Id AND IsDeleted = 0";

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(
                sql,
                new { Id = categoryId },
                _dbSession.Transaction
            );

            return count > 0;
        }

        public async Task<bool> ImageExistAsync(int imageId)
        {
            const string sql = "SELECT COUNT(1) FROM ProductImages WHERE Id = @Id AND IsDeleted = 0";

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(
                sql,
                new { Id = imageId },
                _dbSession.Transaction
            );

            return count > 0;
        }
    }
}