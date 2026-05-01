using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DaimyoDataSolutions.Infrastructure.Data.Repositories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductCategoriesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductCategories> CreateAsync(ProductCategories entity)
        {
            await _dbContext.ProductCategories.AddAsync(entity);
            return entity;
        }

        public async Task<bool> UpdateAsync(ProductCategories entity)
        {
            var existing = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(pc => pc.Id == entity.Id && !pc.IsDeleted);

            if (existing == null) return false;

            _dbContext.Entry(existing).CurrentValues.SetValues(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(ProductCategories entity)
        {
            var existing = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(pc => pc.Id == entity.Id && !pc.IsDeleted);

            if (existing == null) return false;

            existing.IsDeleted = entity.IsDeleted;
            existing.UpdatedBy = entity.UpdatedBy;
            existing.DateUpdated = entity.DateUpdated;

            return true;
        }

        public async Task<ProductCategories?> GetByIdAsync(int id)
        {
            return await _dbContext.ProductCategories
                .AsNoTracking()
                .Include(pc => pc.Product)
                .Include(pc => pc.Categories)
                .FirstOrDefaultAsync(pc => pc.Id == id);
        }

        public async Task<(IEnumerable<ProductCategories> productCategories, int recordCount)> GetAsync(
            ProductCategoriesResourceParameters resourceParameters)
        {
            var query = _dbContext.ProductCategories
                .AsNoTracking()
                .Include(pc => pc.Categories)
                .Where(ps => !ps.IsDeleted);

            if (resourceParameters.ProductId.HasValue)
            {
                query = query.Where(pc => pc.ProductId == resourceParameters.ProductId.Value);
            }

            if (resourceParameters.CategoryId.HasValue)
            {
                query = query.Where(pc => pc.CategoryId == resourceParameters.CategoryId.Value);
            }

            var recordCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(resourceParameters.OrderBy))
            {
                query = resourceParameters.OrderBy.ToLower() switch
                {
                    "id" => query.OrderBy(ps => ps.Id),
                    "id_desc" => query.OrderByDescending(ps => ps.Id),
                    "productid" => query.OrderBy(ps => ps.ProductId),
                    "productid_desc" => query.OrderByDescending(ps => ps.ProductId),
                    "categoryid" => query.OrderBy(ps => ps.CategoryId),
                    "categoryid_desc" => query.OrderByDescending(ps => ps.CategoryId),
                    "datecreated" => query.OrderBy(ps => ps.DateCreated),
                    "datecreated_desc" => query.OrderByDescending(ps => ps.DateCreated),
                    _ => query.OrderByDescending(ps => ps.Id)
                };
            }
            else
            {
                query = query.OrderByDescending(ps => ps.Id);
            }

            // Pagination
            var propertySpecs = await query
                .Skip((resourceParameters.Page - 1) * resourceParameters.PageSize)
                .Take(resourceParameters.PageSize)
                .ToListAsync();

            return (propertySpecs, recordCount);
        }

        public async Task<bool> ExistsAsync(int productId, int categoryId)
        {
            return await _dbContext.ProductCategories
                .AnyAsync(pc => pc.ProductId == productId
                             && pc.CategoryId == categoryId
                             && !pc.IsDeleted);
        }

        public async Task<bool> ExistsAsyncExcludingId(int productId, int categoryId, int excludeId)
        {
            return await _dbContext.ProductCategories
                .AnyAsync(ps => ps.ProductId == productId
                             && ps.CategoryId == categoryId
                             && ps.Id != excludeId
                             && !ps.IsDeleted);
        }
    }
}
