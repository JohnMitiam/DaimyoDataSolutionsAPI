using AutoMapper;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using DaimyoDataSolutions.Application.Services.Base;
using DaimyoDataSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace DaimyoDataSolutions.Application.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductValidator _validator;
        private readonly IProductCategoriesValidator _categoriesValildator;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger,
            IProductValidator validator,
            IProductCategoriesValidator categoriesValidator)
        {
            _validator = validator;
            _categoriesValildator = categoriesValidator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateProductDTO product, string userId)
        {
            var record = _mapper.Map<Products>(product);
            record.CreatedBy = userId;
            record.DateCreated = DateTime.UtcNow;

            var val = _validator.IsValid(record);
            if (!val.isSuccess) return FailedResult(val.errorMessages);

            _unitOfWork.CreateTransaction();

            try
            {
                // 1. Create the Product
                var createdProduct = await _unitOfWork.Products.CreateAsync(record);

                // 2. Handle Categories
                if (product.Categories != null && product.Categories.Any())
                {
                    foreach (var catDto in product.Categories)
                    {
                        var categoryEntity = _mapper.Map<ProductCategories>(catDto);
                        categoryEntity.ProductId = createdProduct.Id; // Ensure FK is set
                        categoryEntity.CreatedBy = userId;
                        categoryEntity.DateCreated = DateTime.UtcNow;

                        var catVal = _categoriesValildator.IsValid(categoryEntity);
                        if (!catVal.isSuccess) throw new Exception("Category validation failed");

                        await _unitOfWork.ProductCategories.CreateAsync(categoryEntity);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return SuccessResult(_mapper.Map<ViewProductDTO>(record));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex.Message);
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO products, string userId)
        {
            try
            {
                // 1. Fetch current record
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null) return FailedResult(ServiceConstants.RecordNotFound);

                // 2. Map basic properties (Name, Price, etc.) from DTO to Entity
                _mapper.Map(products, record);
                record.UpdatedBy = userId;
                record.DateUpdated = DateTime.UtcNow;

                _unitOfWork.CreateTransaction();

                try
                {
                    await _unitOfWork.Products.UpdateAsync(record);
                    await _unitOfWork.SaveChangesAsync();

                    if (products.ProductCategories != null)
                    {
                        await UpdateProductCategoriesAsync(productId, products.ProductCategories, userId);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    var viewProductsDto = _mapper.Map<ViewProductDTO>(products);

                    return SuccessResult(viewProductsDto);
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logger.LogError(ex, "Transaction failed while updating the product");
                }

                // 3. Update main Product table
                //await _unitOfWork.Products.UpdateAsync(record);


                _unitOfWork.Commit();
                return SuccessResult();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($"Update Failed: {ex.Message}");
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> GetAsync(ProductResourceParameters resourceParameters)
        {
            try
            {
                var (products, count) = await _unitOfWork.Products.GetAsync(resourceParameters);
                var dtos = _mapper.Map<IEnumerable<ViewProductDTO>>(products);
                return SuccessResult(new PaginatedList<ViewProductDTO>(dtos.ToList(), count, resourceParameters.Page, resourceParameters.PageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return FailedResult(ServiceConstants.RequestProcessingError);
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
                _logger.LogError($"Error in GetMyProductsAsync: {ex.Message}");
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> GetByIdAsync(int productId)
        {
            try
            {
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                return record == null ? FailedResult(ServiceConstants.RecordNotFound) : SuccessResult(_mapper.Map<ViewProductDTO>(record));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> DeleteAsync(int productId, string userId)
        {
            try
            {
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                record.IsDeleted = true;
                record.UpdatedBy = userId;
                record.DateUpdated = DateTime.UtcNow;

                _unitOfWork.CreateTransaction();

                try
                {
                    var catsParams = new ProductCategoriesResourceParameters
                    {
                        ProductId = productId,
                        Page = 1,
                        PageSize = 1000
                    };
                    var (cats, _) = await _unitOfWork.ProductCategories.GetAsync(catsParams);
                    foreach (var cat in cats) ;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logger.LogError(ex, "Transaction failed while deleting the product!");
                    throw;
                }

                _unitOfWork.Commit();

                return SuccessResult();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        private async Task UpdateProductCategoriesAsync(int productId, List<UpdateProductCategoriesDTO> catsDto, string userId)
        {
            _logger.LogInformation($"Updating specs for product {productId}");

            var existingSpecs = await _unitOfWork.ProductCategories.GetAsync(
                new ProductCategoriesResourceParameters { ProductId = productId, PageSize = 1000 });
            var existingSpecsList = existingSpecs.productCategories.ToList();

            foreach (var catsDtos in catsDto)
            {
                if (catsDtos.Id > 0)
                {
                    var existingSpec = existingSpecsList.FirstOrDefault(cats => cats.Id == catsDtos.CategoriesId);

                    if (existingSpec != null && existingSpec.ProductId == productId)
                    {
                        if (catsDtos.IsDeleted)
                        {
                            existingSpec.IsDeleted = true;
                            existingSpec.UpdatedBy = userId;
                            existingSpec.DateUpdated = DateTime.UtcNow;
                            await _unitOfWork.ProductCategories.DeleteAsync(existingSpec);
                        }
                        else
                        {
                            _mapper.Map(catsDto, existingSpec);
                            existingSpec.IsDeleted = false;
                            existingSpec.UpdatedBy = userId;
                            existingSpec.DateUpdated = DateTime.UtcNow;
                            await _unitOfWork.ProductCategories.UpdateAsync(existingSpec);
                        }
                    }
                    else if (!catsDtos.IsDeleted)
                    {
                        if (!await _unitOfWork.Products.CategoryExistsAsync(catsDtos.CategoriesId))
                        {
                            throw new Exception($"Category with ID {catsDtos.CategoriesId} does not exist.");
                        }

                        var newSpec = _mapper.Map<ProductCategories>(catsDto);
                        newSpec.ProductId = productId;
                        newSpec.CreatedBy = userId;
                        newSpec.DateCreated = DateTime.UtcNow;
                        newSpec.IsDeleted = false;

                        await _unitOfWork.ProductCategories.CreateAsync(newSpec);
                    }
                }
                else
                {
                    var existingBySpecId = existingSpecsList.FirstOrDefault(cats => cats.CategoryId == catsDtos.CategoriesId);

                    if (existingBySpecId != null)
                    {
                        if (catsDtos.IsDeleted)
                        {
                            existingBySpecId.IsDeleted = true;
                            existingBySpecId.UpdatedBy = userId;
                            existingBySpecId.DateUpdated = DateTime.UtcNow;
                            await _unitOfWork.ProductCategories.DeleteAsync(existingBySpecId);
                        }
                        else
                        {
                            if (existingBySpecId.IsDeleted)
                            {
                                if (!await _unitOfWork.Products.CategoryExistsAsync(catsDtos.CategoriesId))
                                {
                                    throw new Exception($"Category with ID {catsDtos.CategoriesId} does not exist.");
                                }

                                var newSpec = _mapper.Map<ProductCategories>(catsDtos);
                                newSpec.ProductId = productId;
                                newSpec.CreatedBy = userId;
                                newSpec.DateCreated = DateTime.UtcNow;
                                newSpec.IsDeleted = false;

                                await _unitOfWork.ProductCategories.CreateAsync(newSpec);
                            }
                            else
                            {
                                _mapper.Map(catsDtos, existingBySpecId);
                                existingBySpecId.UpdatedBy = userId;
                                existingBySpecId.DateUpdated = DateTime.UtcNow;
                                await _unitOfWork.ProductCategories.UpdateAsync(existingBySpecId);
                            }
                        }
                    }
                    else if (!catsDtos.IsDeleted)
                    {
                        if (!await _unitOfWork.Products.CategoryExistsAsync(catsDtos.CategoriesId))
                        {
                            throw new Exception($"Category with ID {catsDtos.CategoriesId} does not exist.");
                        }

                        var newSpec = _mapper.Map<ProductCategories>(catsDtos);
                        newSpec.ProductId = productId;
                        newSpec.CreatedBy = userId;
                        newSpec.DateCreated = DateTime.UtcNow;
                        newSpec.IsDeleted = false;

                        await _unitOfWork.ProductCategories.CreateAsync(newSpec);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var freshSpecs = await _unitOfWork.ProductCategories.GetAsync(
                new ProductCategoriesResourceParameters { ProductId = productId, PageSize = 1000 });
            var activeSpecs = freshSpecs.productCategories.Where(spec => !spec.IsDeleted).ToList();


            _logger.LogInformation($"Finished updating specs for product {productId}");
        }
    }
}