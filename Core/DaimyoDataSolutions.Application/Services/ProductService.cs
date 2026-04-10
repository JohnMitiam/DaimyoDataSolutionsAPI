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
                var record = _mapper.Map<Product>(product);
                //record.CreatedBy = affiliateId;
                record.DateCreated = DateTime.UtcNow;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Products.CreateAsync(record);

                _unitOfWork.Commit();

                return SuccessResult(_mapper.Map<ViewProductDTO>(record));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> DeleteAsync(int productId)
        {
            try
            {
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                //var validationResult = await _validator.IsValidForDeleteAsync(record);
                //if (!validationResult.isSuccess)
                //{
                //    return FailedResult(validationResult.errorMessages);
                //}

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Products.DeleteAsync(record.Id);

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

        public async Task<IServiceResult> GetAsync(ProductResourceParameters resourceParameters)
        {
            try
            {
                var result = await _unitOfWork.Products.GetAsync(resourceParameters).ConfigureAwait(false);

                var paginatedResult = new PaginatedList<ViewProductDTO>(
                    _mapper.Map<IEnumerable<ViewProductDTO>>(result.products).ToList(),
                    result.recordCount,
                    resourceParameters.Page,
                    resourceParameters.PageSize);

                return SuccessResult(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> GetByIdAsync(int productId)
        {
            try
            {
                var record = await _unitOfWork.Products.GetByIdAsync(productId);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                var result = _mapper.Map<ViewProductDTO>(record);

                return SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> UpdateAsync(int productId, UpdateProductDTO products)
        {
            try
            {
                var record = await _unitOfWork.Products.GetByIdAsync(productId).ConfigureAwait(false);
                if (record == null)
                    return FailedResult(ServiceConstants.RecordNotFound);

                _mapper.Map(products, record);
                record.DateUpdated = DateTime.UtcNow;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();
                await _unitOfWork.Products.UpdateAsync(record).ConfigureAwait(false);

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

    }
}
