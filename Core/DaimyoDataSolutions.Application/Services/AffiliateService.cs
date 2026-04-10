using AutoMapper;
using Microsoft.Extensions.Logging;
using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using DaimyoDataSolutions.Application.Services.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Services
{
    public class AffiliateService : BaseService, IAffiliateService
    {
        private readonly IAffiliateValidator _validator;
        private readonly ILogger<AffiliateService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AffiliateService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AffiliateService> logger, IAffiliateValidator validator)
        {
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateAffiliateDTO affiliate)
        {
            try
            {
                var record = _mapper.Map<Affiliate>(affiliate);
                //record.CreatedBy = affiliateId;
                record.DateCreated = DateTime.UtcNow;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Affiliate.CreateAsync(record);

                _unitOfWork.Commit();

                return SuccessResult(_mapper.Map<ViewAffiliateDTO>(record));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> DeleteAsync(int id)
        {
            try
            {
                var record = await _unitOfWork.Affiliate.GetByIdAsync(id);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Affiliate.DeleteAsync(record.Id);

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

        public async Task<IServiceResult> GetAsync(AffiliateResourceParameters resourceParameters)
        {
            try
            {
                var result = await _unitOfWork.Affiliate.GetAsync(resourceParameters).ConfigureAwait(false);

                var paginatedResult = new PaginatedList<ViewAffiliateDTO>(
                    _mapper.Map<IEnumerable<ViewAffiliateDTO>>(result.affiliate).ToList(),
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
                var record = await _unitOfWork.Affiliate.GetByIdAsync(productId);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                var result = _mapper.Map<ViewAffiliateDTO>(record);

                return SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> UpdateAsync(int affiliateId, UpdateAffiliateDTO affiliate)
        {
            try
            {
                var record = await _unitOfWork.Affiliate.GetByIdAsync(affiliateId).ConfigureAwait(false);
                if (record == null)
                    return FailedResult(ServiceConstants.RecordNotFound);

                _mapper.Map(affiliate, record);
                record.DateUpdated = DateTime.UtcNow;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();
                await _unitOfWork.Affiliate.UpdateAsync(record).ConfigureAwait(false);

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
