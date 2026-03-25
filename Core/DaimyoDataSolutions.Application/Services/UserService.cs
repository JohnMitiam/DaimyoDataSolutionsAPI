using AutoMapper;
using Microsoft.Extensions.Logging;
using DaimyoDataSolutions.Application.DTOs.User;
using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using DaimyoDataSolutions.Application.Services.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserValidator _validator;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserValidator validator, ILogger<UserService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateAsync(CreateUserDTO user, string userId)
        {
            try
            {
                var record = _mapper.Map<User>(user);
                record.CreatedBy = userId;
                record.DateCreated = DateTime.UtcNow;

                var validationResult = _validator.IsValid(record);
                if (!validationResult.isSuccess)
                {
                    return FailedResult(validationResult.errorMessages);
                }

                _unitOfWork.CreateTransaction();

                await _unitOfWork.Users.CreateAsync(record);

                _unitOfWork.Commit();

                return SuccessResult(_mapper.Map<ViewUserDTO>(record));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> DeleteAsync(int id, string userId)
        {
            try
            {
                var record = await _unitOfWork.Users.GetByIdAsync(id);
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

                await _unitOfWork.Users.DeleteAsync(record.Id);

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

        public async Task<IServiceResult> GetAsync(UserResourceParameters resourceParameters)
        {
            try
            {
                var result = await _unitOfWork.Users.GetAsync(resourceParameters).ConfigureAwait(false);

                var paginatedResult = new PaginatedList<ViewUserDTO>(
                    _mapper.Map<IEnumerable<ViewUserDTO>>(result.users).ToList(),
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
                var record = await _unitOfWork.Users.GetByIdAsync(productId);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                var result = _mapper.Map<ViewUserDTO>(record);

                return SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"{ex.Message}");

                return FailedResult(ServiceConstants.RequestProcessingError);
            }
        }

        public async Task<IServiceResult> UpdateAsync(int uId, UpdateUserDTO users, string userId)
        {
            try
            {
                var record = await _unitOfWork.Users.GetByIdAsync(uId).ConfigureAwait(false);
                if (record == null)
                {
                    return FailedResult(ServiceConstants.RecordNotFound);
                }

                return SuccessResult(_mapper.Map<ViewUserDTO>(record));
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
