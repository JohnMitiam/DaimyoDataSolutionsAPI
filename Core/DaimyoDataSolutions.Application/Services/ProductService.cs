using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using DaimyoDataSolutions.Application.Services.Base;
using DaimyoDataSolutions.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DaimyoDataSolutions.Application.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductValidator _productValidator;
        private readonly IProductCategoriesValidator _categoryValidator;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductValidator productValidator,
            IProductCategoriesValidator categoryValidator)
        {
            _productValidator = productValidator;
            _categoryValidator = categoryValidator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateProductDTO productDto, string userId)
        {
            var productEntity = _mapper.Map<Products>(productDto);
            productEntity.CreatedBy = userId;
            productEntity.DateCreated = DateTime.UtcNow;

            // 1. Validate Product
            var validation = _productValidator.IsValid(productEntity);
            if (!validation.isSuccess) return FailedResult(validation.errorMessages);

            _unitOfWork.CreateTransaction();
            try
            {
                // 2. Create Product first to get the ID
                var createdProduct = await _unitOfWork.Products.CreateAsync(productEntity);
                await _unitOfWork.SaveChangesAsync();

                // 3. Handle Categories
                if (productDto.Categories != null && productDto.Categories.Any())
                {
                    foreach (var catDto in productDto.Categories)
                    {
                        var categoryInfo = await _unitOfWork.Categories.GetByIdAsync(catDto.CategoryId);

                        var productCategory = new ProductCategories
                        {
                            ProductId = createdProduct.Id, // Use the new ID
                            CategoryId = catDto.CategoryId,
                            CategoryName = categoryInfo.Name,
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow
                        };

                        await _unitOfWork.ProductCategories.CreateAsync(productCategory);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                _unitOfWork.Commit();

                var result = await _unitOfWork.Products.GetByIdAsync(createdProduct.Id);
                return SuccessResult(_mapper.Map<ViewProductDTO>(result));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex, "Error creating product with categories");
                return FailedResult($"Error: {ex.Message}");
            }
        }

        public async Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO productDto, string userId)
        {
            try
            {
                // 1. Fetch the existing product record
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null) return FailedResult("Product not found.");

                // 2. Map DTO changes to the entity
                _mapper.Map(productDto, record);
                record.UpdatedBy = userId;
                record.DateUpdated = DateTime.UtcNow;

                // 3. Validate the Product Entity
                var productValidation = _productValidator.IsValid(record);
                if (!productValidation.isSuccess)
                {
                    return FailedResult(productValidation.errorMessages);
                }

                _unitOfWork.CreateTransaction();

                try
                {
                    // 4. Update Product Basic Info
                    await _unitOfWork.Products.UpdateAsync(record);

                    // 5. Sync Categories (Delete missing, Add new, Update existing)
                    if (productDto.ProductCategories != null)
                    {
                        await UpdateProductCategoriesAsync(productId, productDto.ProductCategories, userId);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    // 6. Return the refreshed data
                    var result = await _unitOfWork.Products.GetByIdAsync(productId);
                    return SuccessResult(_mapper.Map<ViewProductDTO>(result));
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logger.LogError(ex, "Transaction failed while updating product {ProductId}", productId);
                    return FailedResult("An error occurred during the update transaction.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update failed for product {ProductId}", productId);
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> GetAsync(ProductResourceParameters resourceParameters)
        {
            try
            {
                var result = await _unitOfWork.Products.GetAsync(resourceParameters);
                var productDtos = _mapper.Map<IEnumerable<ViewProductDTO>>(result.products).ToList();

                var pagedResult = new PaginatedList<ViewProductDTO>(
                        productDtos,
                        result.recordCount,
                        resourceParameters.Page,
                        resourceParameters.PageSize
                    );

                return SuccessResult(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return FailedResult("An error occured while fetching products");
            }
        }

        public async Task<IServiceResult> GetMyProductsAsync(string userId)
        {
            try
            {
                var (products, count) = await _unitOfWork.Products.GetMyProductAsync(userId);

                var productDtos = _mapper.Map<IEnumerable<ViewProductDTO>>(products).ToList();

                return SuccessResult(productDtos);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error fetching managed products for user {UserId}", userId);
                return FailedResult("An error occurred while fetching your managed products.");
            }
        }

        public async Task<IServiceResult> GetByIdAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return FailedResult("Product not found.");
                }

                var productDto = _mapper.Map<ViewProductDTO>(product);

                return SuccessResult(productDto);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error fetching product");
                return FailedResult("An error occurred while fetching the product.");
            }
        }

        public async Task<IServiceResult> DeleteAsync(int productId, string userId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return FailedResult("Product not found. ");
                }

                product.IsDeleted = true;
                product.UpdatedBy = userId;
                product.DateUpdated = DateTime.UtcNow;

                _unitOfWork.CreateTransaction();

                try
                {
                    var categoryParams = new ProductCategoriesResourceParameters
                    {
                        ProductId = productId,
                        Page = 1,
                        PageSize = 1000
                    };
                    var (categories, _) = await _unitOfWork.ProductCategories.GetAsync(categoryParams);
                    foreach (var category in categories)
                    {
                        category.IsDeleted = true;
                        category.UpdatedBy = userId;
                        category.DateUpdated = DateTime.UtcNow;
                        await _unitOfWork.ProductCategories.DeleteAsync(category);
                    }

                    await _unitOfWork.Products.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    return SuccessResult();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logger.LogError(ex, "Transaction failed while deleting the product!");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        private async Task UpdateProductCategoriesAsync(int productId, List<UpdateProductCategoriesDTO> dtoCats, string userId)
        {
            // 1. Get current categories from DB for this product
            var existingData = await _unitOfWork.ProductCategories.GetAsync(
                new ProductCategoriesResourceParameters { ProductId = productId, PageSize = 1000 });

            var dbCategories = existingData.productCategories.ToList();

            // 2. Identify what to REMOVE (In DB but not in the new DTO list)
            var incomingCategoryIds = dtoCats.Where(d => !d.IsDeleted).Select(d => d.CategoryId).ToList();
            var toDelete = dbCategories.Where(db => !incomingCategoryIds.Contains(db.CategoryId)).ToList();

            foreach (var cat in toDelete)
            {
                cat.IsDeleted = true;
                cat.UpdatedBy = userId;
                cat.DateUpdated = DateTime.UtcNow;
                await _unitOfWork.ProductCategories.DeleteAsync(cat);
            }

            // 3. Identify what to ADD or REACTIVATE
            foreach (var dto in dtoCats.Where(d => !d.IsDeleted))
            {
                var existingInDb = dbCategories.FirstOrDefault(db => db.CategoryId == dto.CategoryId);

                if (existingInDb == null)
                {
                    // Fully new link - Check if category exists first
                    if (!await _unitOfWork.Products.CategoryExistsAsync(dto.CategoryId))
                        throw new Exception($"Category {dto.CategoryId} does not exist.");

                    var newLink = new ProductCategories
                    {
                        ProductId = productId,
                        CategoryId = dto.CategoryId,
                        CreatedBy = userId,
                        DateCreated = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await _unitOfWork.ProductCategories.CreateAsync(newLink);
                }
                else if (existingInDb.IsDeleted)
                {
                    // Reactivate an old link that was previously soft-deleted
                    existingInDb.IsDeleted = false;
                    existingInDb.UpdatedBy = userId;
                    existingInDb.DateUpdated = DateTime.UtcNow;
                    await _unitOfWork.ProductCategories.UpdateAsync(existingInDb);
                }
            }
        }
    }
}