using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class ProductImagesRepository : IProductImagesRepository
    {
        private readonly DatabaseSession _dbSession;

        public ProductImagesRepository(DatabaseSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<ProductImages> CreateAsync(ProductImages productImages)
        {
            const string query = "sp_CreateProductImages";

            var queryparams = new
            {
                ProductId = productImages.ProductId,
                ImageData = productImages.ImageData,
                MimeType = productImages.MimeType,
                IsPrimary = productImages.IsPrimary,
                CreatedBy = productImages.CreatedBy,
                DateCreated = productImages.DateCreated,
                IsDeleted = productImages.IsDeleted
            };

            productImages.Id = await _dbSession.Connection.ExecuteScalarAsync<int>(
                query,
                queryparams,
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
            );

            return productImages;
        }

        public async Task<bool> UpdateAsync(ProductImages productImages)
        {
            const string query = "sp_UpdateProductImages";

            var queryParams = new
            {
                ID = productImages.Id,
                ProductId = productImages.ProductId,
                ImageData = productImages.ImageData,
                MimeType = productImages.MimeType,
                IsPrimary = productImages.IsPrimary,
                UpdatedBy = productImages.UpdatedBy,
                DateUpdated = productImages.DateUpdated
            };

            await _dbSession.Connection.ExecuteAsync(
                query,
                queryParams,
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
            ).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> DeleteAsync(ProductImages productImages)
        {
            const string query = "sp_DeleteProductImages";

            var queryParams = new
            {
                ProductImageID = productImages.Id
            };

            await _dbSession.Connection.ExecuteAsync(
                query,
                queryParams,
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<ProductImages?> GetByIdAsync(int productImagesId)
        {
            const string query = "sp_GetProductImagesById";

            var queryParams = new
            {
                ID = productImagesId
            };

            return await _dbSession.Connection.QueryFirstOrDefaultAsync<ProductImages>(
                query,
                queryParams,
                _dbSession.Transaction,
                commandType: CommandType.StoredProcedure
            ).ConfigureAwait(false);
        }

        public async Task<(IEnumerable<ProductImages> productImages, int recordCount)> GetAsync(
            ProductImagesResourceParameters resourceParameters)
        {
            var queryParamBuilder = new QueryParameters(
                resourceParameters.Search ?? string.Empty,
                resourceParameters.SearchFields ?? new List<string>(),
                resourceParameters.Page,
                resourceParameters.PageSize
            );

            var baseDataQuery = "SELECT * FROM ProductImages WHERE IsDeleted = 0 ";
            var baseCountQuery = "SELECT COUNT(*) FROM ProductImages WHERE IsDeleted = 0 ";

            var searchSQL = queryParamBuilder.GetSearchSQLQuery();
            var filterSQL = queryParamBuilder.GetFilterSQLQuery();
            var paginationSQL = queryParamBuilder.GetPaginationSQLQuery();

            var finalDataQuery = baseDataQuery + searchSQL + filterSQL + paginationSQL;
            var finalCountQuery = baseCountQuery + searchSQL + filterSQL;

            var result = await _dbSession.Connection.QueryAsync<ProductImages>(finalDataQuery, queryParamBuilder.Parameters);
            var totalCount = await _dbSession.Connection.ExecuteScalarAsync<int>(finalCountQuery, queryParamBuilder.Parameters);

            return (result, totalCount);
        }

        public async Task<bool> PrimaryImageExistsAsync(int productId, int excludeId)
        {
            const string query = @"SELECT COUNT(1) FROM ProductImages
                                 WHERE ProductId = @ProductId
                                 AND Id != @ExcludeId
                                 AND IsDeleted = 0
                                 AND IsPrimary = 1";

            var queryParams = new
            {
                ProductId = productId,
                ExcludeId = excludeId
            };

            var count = await _dbSession.Connection.ExecuteScalarAsync<int>(
                query,
                queryParams,
                _dbSession.Transaction
            );

            return count > 0;
        }
    }
}