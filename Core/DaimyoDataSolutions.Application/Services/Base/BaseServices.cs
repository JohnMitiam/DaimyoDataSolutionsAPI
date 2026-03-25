using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.Application.Services.Base
{
    public abstract class BaseService
    {
        protected ServiceResult Result(bool isSuccess, string? message = null, List<string> messages = null)
        {
            var errorMessages = messages ?? new List<string>();
            if (!string.IsNullOrEmpty(message))
            {
                errorMessages.Add(message);
            }

            return new ServiceResult { IsSuccess = isSuccess, ErrorMessages = errorMessages };
        }

        protected ServiceResult<T> Result<T>(T data, bool isSuccess, string? message = null, List<string> messages = null)
        {
            var errorMessages = messages ?? new List<string>();
            if (!string.IsNullOrEmpty(message))
            {
                errorMessages.Add(message);
            }

            return new ServiceResult<T> { IsSuccess = isSuccess, ErrorMessages = errorMessages, Data = data };
        }

        protected ServiceResult FailedResult(string message)
        {
            return Result(false, message);
        }

        protected ServiceResult FailedResult(List<string> messages)
        {
            return Result(false, null, messages);
        }

        protected ServiceResult SuccessResult()
        {
            return Result(true);
        }

        protected ServiceResult<T> SuccessResult<T>(T data)
        {
            return Result<T>(data, true);
        }
    }
}
