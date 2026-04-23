using AutoMapper;
using Microsoft.Extensions.Logging;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using DaimyoDataSolutions.Application.Services.Base;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.DTOs.Product;

namespace DaimyoDataSolutions.Application.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductValidator _validator;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger, IProductValidator validator)
        {
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateProductDTO product)
        {
            try
            {
                var record = _mapper.Map<Products>(product);
                record.DateCreated = DateTime.UtcNow;

                var val = _validator.IsValid(record);
                if (!val.isSuccess) return FailedResult(val.errorMessages);

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Products.CreateAsync(record);

                if (product.ProductCategoryIds?.Any() == true)
                {
                    // We still pass the list of ints to the repo's Add method
                    await _unitOfWork.Products.AddProductCategoriesAsync(record.Id, product.ProductCategoryIds);
                }

                _unitOfWork.Commit();

                // Refresh to get the names for the response
                var completedRecord = await _unitOfWork.Products.GetByIdAsync(record.Id);
                return SuccessResult(_mapper.Map<ViewProductDTO>(completedRecord));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex.Message);
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO products)
        {
            try
            {
                // 1. Fetch current record
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null) return FailedResult(ServiceConstants.RecordNotFound);

                // 2. Map basic properties (Name, Price, etc.) from DTO to Entity
                _mapper.Map(products, record);
                record.DateUpdated = DateTime.UtcNow;

                _unitOfWork.CreateTransaction();

                // 3. Update main Product table
                await _unitOfWork.Products.UpdateAsync(record);


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

        public async Task<IServiceResult> DeleteAsync(int productId)
        {
            try
            {
                _unitOfWork.CreateTransaction();
                await _unitOfWork.Products.DeleteAsync(productId);
                _unitOfWork.Commit();
                return SuccessResult();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex.Message);
                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }
    }
}