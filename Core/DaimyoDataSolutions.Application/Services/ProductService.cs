using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.DTOs.ProductImages;
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
        private readonly IProductImagesValidator _imageValidator;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductValidator productValidator,
            IProductCategoriesValidator categoryValidator,
            IProductImagesValidator imagesValidator)
        {
            _productValidator = productValidator;
            _categoryValidator = categoryValidator;
            _imageValidator = imagesValidator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateProductDTO productDto, string userId)
        {
            var productEntity = _mapper.Map<Products>(productDto);
            productEntity.CreatedBy = userId;
            productEntity.DateCreated = DateTime.UtcNow;

            var validation = _productValidator.IsValid(productEntity);
            if (!validation.isSuccess) return FailedResult(validation.errorMessages);

            _unitOfWork.CreateTransaction();
            try
            {
                var createdProduct = await _unitOfWork.Products.CreateAsync(productEntity);
                await _unitOfWork.SaveChangesAsync();

                if (productDto.Categories != null && productDto.Categories.Any())
                {
                    foreach (var catDto in productDto.Categories)
                    {
                        var categoryInfo = await _unitOfWork.Categories.GetByIdAsync(catDto.CategoryId);

                        var productCategory = new ProductCategories
                        {
                            ProductId = createdProduct.Id,
                            CategoryId = catDto.CategoryId,
                            CategoryName = categoryInfo.Name,
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow
                        };

                        await _unitOfWork.ProductCategories.CreateAsync(productCategory);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                if (productDto.Images != null && productDto.Images.Any())
                {
                    foreach (var imageDto in productDto.Images)
                    {
                        var productImage = new ProductImages
                        {
                            ProductId = createdProduct.Id,
                            ImageData = imageDto.ImageData,
                            MimeType = imageDto.MimeType,
                            IsPrimary = imageDto.IsPrimary,
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow
                        };

                        await _unitOfWork.ProductImages.CreateAsync(productImage);
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
                _logger.LogError(ex, "Error creating product");
                return FailedResult($"Error: {ex.Message}");
            }
        }

        public async Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO productDto, string userId)
        {
            try
            {
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null) return FailedResult("Product not found.");

                _mapper.Map(productDto, record);
                record.UpdatedBy = userId;
                record.DateUpdated = DateTime.UtcNow;

                var productValidation = _productValidator.IsValid(record);
                if (!productValidation.isSuccess)
                {
                    return FailedResult(productValidation.errorMessages);
                }

                _unitOfWork.CreateTransaction();

                try
                {
                    await _unitOfWork.Products.UpdateAsync(record);

                    if (productDto.ProductCategories != null)
                    {
                        await UpdateProductCategoriesAsync(productId, productDto.ProductCategories, userId);
                    }

                    if (productDto.PropertyImages != null)
                    {
                        await UpdateProductImagesAsync(productId, productDto.PropertyImages, userId);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching managed products for user {UserId}", userId);
                return FailedResult("An error occurred while fetching your managed products.");
            }
        }

        public async Task<IServiceResult> GetByIdAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null) return FailedResult("Product not found.");

                var productDto = _mapper.Map<ViewProductDTO>(product);
                return SuccessResult(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product");
                return FailedResult("An error occurred while fetching the product.");
            }
        }
        public async Task<IServiceResult> DeleteAsync(int productId, string userId)
        {
            try
            {
                // 1. Verify the product exists first
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    return FailedResult("Product not found.");
                }

                _unitOfWork.CreateTransaction();

                try
                {
                    // 2. Fetch and Soft Delete Categories related ONLY to this product
                    var categoryParams = new ProductCategoriesResourceParameters
                    {
                        ProductId = productId,
                        Page = 1,
                        PageSize = 1000 // Ensure we get all links for this product
                    };

                    var (categories, _) = await _unitOfWork.ProductCategories.GetAsync(categoryParams);

                    foreach (var category in categories)
                    {
                        // CRITICAL SAFETY CHECK: Ensure the repository didn't return other products' data
                        if (category.ProductId == productId)
                        {
                            category.IsDeleted = true;
                            category.UpdatedBy = userId;
                            category.DateUpdated = DateTime.UtcNow;
                            await _unitOfWork.ProductCategories.DeleteAsync(category);
                        }
                    }

                    // 3. Fetch and Soft Delete Images related ONLY to this product
                    var imageParams = new ProductImagesResourceParameters
                    {
                        ProductId = productId,
                        Page = 1,
                        PageSize = 1000
                    };

                    var (images, _) = await _unitOfWork.ProductImages.GetAsync(imageParams);

                    foreach (var image in images)
                    {
                        // CRITICAL SAFETY CHECK: Prevent accidental deletion of images from other products
                        if (image.ProductId == productId)
                        {
                            image.IsDeleted = true;
                            image.UpdatedBy = userId;
                            image.DateUpdated = DateTime.UtcNow;
                            await _unitOfWork.ProductImages.DeleteAsync(image);
                        }
                    }

                    // 4. Soft Delete the main Product record
                    product.IsDeleted = true;
                    product.UpdatedBy = userId;
                    product.DateUpdated = DateTime.UtcNow;

                    // We pass the whole object so the Repo can extract the ID and Audit fields
                    await _unitOfWork.Products.DeleteAsync(product);

                    // 5. Finalize the Database Transaction
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    return SuccessResult();
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any of the steps above fail
                    _unitOfWork.Rollback();
                    _logger.LogError(ex, "Transaction failed while deleting product {ProductId}", productId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed for product {ProductId}", productId);
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        //public async Task<IServiceResult> DeleteAsync(int productId, string userId)
        //{
        //    try
        //    {
        //        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        //        if (product == null) return FailedResult("Product not found. ");

        //        product.IsDeleted = true;
        //        product.UpdatedBy = userId;
        //        product.DateUpdated = DateTime.UtcNow;

        //        _unitOfWork.CreateTransaction();

        //        try
        //        {
        //            var categoryParams = new ProductCategoriesResourceParameters
        //            {
        //                ProductId = productId,
        //                Page = 1,
        //                PageSize = 1000
        //            };

        //            var (categories, _) = await _unitOfWork.ProductCategories.GetAsync(categoryParams);
        //            foreach (var category in categories)
        //            {
        //                category.IsDeleted = true;
        //                category.UpdatedBy = userId;
        //                category.DateUpdated = DateTime.UtcNow;
        //                await _unitOfWork.ProductCategories.DeleteAsync(category);
        //            }

        //            var imageParams = new ProductImagesResourceParameters
        //            {
        //                ProductId = productId,
        //                Page = 1,
        //                PageSize = 1000
        //            };

        //            var (images, _) = await _unitOfWork.ProductImages.GetAsync(imageParams);
        //            foreach (var image in images)
        //            {
        //                image.IsDeleted = true;
        //                image.UpdatedBy = userId;
        //                image.DateUpdated = DateTime.UtcNow;
        //                await _unitOfWork.ProductImages.DeleteAsync(image);
        //            }

        //            await _unitOfWork.Products.DeleteAsync(product);
        //            await _unitOfWork.SaveChangesAsync();
        //            _unitOfWork.Commit();

        //            return SuccessResult();
        //        }
        //        catch (Exception ex)
        //        {
        //            _unitOfWork.Rollback();
        //            _logger.LogError(ex, "Transaction failed while deleting the product!");
        //            throw;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _unitOfWork.Rollback();
        //        _logger.LogError($"{ex.Message}");
        //        return FailedResult(ServiceConstants.RequestProcessingError);
        //    }
        //}

        private async Task UpdateProductCategoriesAsync(int productId, List<UpdateProductCategoriesDTO> dtoCats, string userId)
        {
            var existingData = await _unitOfWork.ProductCategories.GetAsync(
                new ProductCategoriesResourceParameters { ProductId = productId, PageSize = 1000 });

            var dbCategories = existingData.productCategories.ToList();

            // Identify what to REMOVE
            var incomingCategoryIds = dtoCats.Where(d => !d.IsDeleted).Select(d => d.CategoryId).ToList();
            var toDelete = dbCategories.Where(db => !incomingCategoryIds.Contains(db.CategoryId)).ToList();

            foreach (var cat in toDelete)
            {
                cat.IsDeleted = true;
                cat.UpdatedBy = userId;
                cat.DateUpdated = DateTime.UtcNow;
                await _unitOfWork.ProductCategories.DeleteAsync(cat);
            }

            // Identify what to ADD or REACTIVATE
            foreach (var dto in dtoCats.Where(d => !d.IsDeleted))
            {
                var existingInDb = dbCategories.FirstOrDefault(db => db.CategoryId == dto.CategoryId);

                if (existingInDb == null)
                {
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
                    existingInDb.IsDeleted = false;
                    existingInDb.UpdatedBy = userId;
                    existingInDb.DateUpdated = DateTime.UtcNow;
                    await _unitOfWork.ProductCategories.UpdateAsync(existingInDb);
                }
            }
        }

        private async Task UpdateProductImagesAsync(int productId, List<UpdateProductImagesDTO> dtoImgs, string userId)
        {
            var existingData = await _unitOfWork.ProductImages.GetAsync(
                new ProductImagesResourceParameters { ProductId = productId, PageSize = 1000 });

            var dbImages = existingData.productImages.ToList();

            // 1. Identify the new Primary ImageData from the DTOs
            var newPrimaryImageData = dtoImgs.FirstOrDefault(d => d.IsPrimary && !d.IsDeleted)?.ImageData;

            // 2. Identify what to REMOVE
            var incomingImageDatas = dtoImgs.Where(d => !d.IsDeleted).Select(d => d.ImageData).ToList();
            var toDelete = dbImages.Where(db => !incomingImageDatas.Contains(db.ImageData)).ToList();

            foreach (var img in toDelete)
            {
                img.IsDeleted = true;
                img.IsPrimary = false; // Ensure deleted images aren't primary
                img.UpdatedBy = userId;
                img.DateUpdated = DateTime.UtcNow;
                await _unitOfWork.ProductImages.DeleteAsync(img);
            }

            // 3. Identify what to ADD or REACTIVATE
            foreach (var dto in dtoImgs.Where(d => !d.IsDeleted))
            {
                var existingInDb = dbImages.FirstOrDefault(db => db.ImageData == dto.ImageData);

                if (existingInDb == null)
                {
                    var newImage = new ProductImages
                    {
                        ProductId = productId,
                        ImageData = dto.ImageData,
                        MimeType = dto.MimeType,
                        IsPrimary = dto.ImageData == newPrimaryImageData,
                        CreatedBy = userId,
                        DateCreated = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await _unitOfWork.ProductImages.CreateAsync(newImage);
                }
                else
                {
                    // Update existing record (Reactivate if soft-deleted)
                    existingInDb.IsDeleted = false;
                    existingInDb.IsPrimary = existingInDb.ImageData == newPrimaryImageData;

                    existingInDb.UpdatedBy = userId;
                    existingInDb.DateUpdated = DateTime.UtcNow;
                    await _unitOfWork.ProductImages.UpdateAsync(existingInDb);
                }
            }
        }
    }
}