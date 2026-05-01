using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
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
                CreatedBy = product.CreatedBy,
                DateCreated = product.DateCreated,
                IsDeleted = product.IsDeleted,
            };

            product.Id = await _dbSession.Connection.ExecuteScalarAsync<int>(query, queryParams, _dbSession.Transaction, commandType: CommandType.StoredProcedure);

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

            var baseQuery = "FROM Products WHERE IsDeleted = 0 ";
            var dataSql = "SELECT * " + baseQuery + queryParamBuilder.GetSearchSQLQuery() + queryParamBuilder.GetFilterSQLQuery() + queryParamBuilder.GetPaginationSQLQuery();
            var countSql = "SELECT COUNT(*) " + baseQuery + queryParamBuilder.GetSearchSQLQuery() + queryParamBuilder.GetFilterSQLQuery();

            var products = (await _dbSession.Connection.QueryAsync<Products>(dataSql, queryParamBuilder.Parameters)).ToList();
            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(countSql, queryParamBuilder.Parameters);

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

            return (products, count);
        }

        public async Task<Products?> GetByIdAsync(int productId)
        {
            var sql = @$"sp_GetProductById";

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
                        pc.Categories = category;
                        currentProduct.ProductCategories.Add(pc);
                    }
                    return currentProduct;
                },
                new { ID = productId },
                _dbSession.Transaction,
                splitOn: "Id,Id"); // Adjust "splitOn" based on your actual column names

            return result.FirstOrDefault();
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


        public async Task<bool> DeleteAsync(int id)
        {
            var query = @"sp_DeleteProduct";

            // ExecuteAsync returns the number of rows affected
            var rowsAffected = await _dbSession.Connection.ExecuteAsync(
                query,
                new { ProductID = id },
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Products product)
        {
            return await DeleteAsync(product.Id);
        }

        public async Task<bool> CategoryExistsAsync(int specificationId)
        {
            var sql = "SELECT COUNT(1) FROM Categories WHERE Id = @Id AND IsDeleted = 0";

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(
                sql,
                new { Id = specificationId },
                _dbSession.Transaction
            );

            return count > 0;
        }
    }
}