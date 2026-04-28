using AutoMapper;
using Microsoft.Extensions.Logging;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using DaimyoDataSolutions.Application.Services.Base;
using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Application.DTOs.Category;

namespace DaimyoDataSolutions.Application.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryValidator _validator;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger, ICategoryValidator validator)
        {
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateCategoryDTO Category, string userId)
        {
            try
            {
                var record = _mapper.Map<Category>(Category);
                //record.CreatedBy = affiliateId;
                record.CreatedBy = userId;
                record.DateCreated = DateTime.UtcNow;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Categories.CreateAsync(record);

                _unitOfWork.Commit();

                return SuccessResult(_mapper.Map<ViewCategoryDTO>(record));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> DeleteAsync(int CategoryId, string userId)
        {
            try
            {
                var record = await _unitOfWork.Categories.GetByIdAsync(CategoryId);
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

                await _unitOfWork.Categories.DeleteAsync(record.Id);

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

        public async Task<IServiceResult> GetAsync(CategoryResourceParameters resourceParameters)
        {
            try
            {
                var result = await _unitOfWork.Categories.GetAsync(resourceParameters).ConfigureAwait(false);

                var paginatedResult = new PaginatedList<ViewCategoryDTO>(
                    _mapper.Map<IEnumerable<ViewCategoryDTO>>(result.categories).ToList(),
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

        public async Task<IServiceResult> GetByIdAsync(int CategoryId)
        {
            try
            {
                var record = await _unitOfWork.Categories.GetByIdAsync(CategoryId);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                var result = _mapper.Map<ViewCategoryDTO>(record);

                return SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> UpdateAsync(int CategoryId, UpdateCategoryDTO Categories, string userId)
        {
            try
            {
                var record = await _unitOfWork.Categories.GetByIdAsync(CategoryId).ConfigureAwait(false);
                if (record == null)
                    return FailedResult(ServiceConstants.RecordNotFound);

                _mapper.Map(Categories, record);
                record.DateUpdated = DateTime.UtcNow;
                record.UpdatedBy = userId;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();
                await _unitOfWork.Categories.UpdateAsync(record).ConfigureAwait(false);

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
