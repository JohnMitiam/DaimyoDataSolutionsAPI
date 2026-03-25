using DaimyoDataSolutions.Application.Services.Base;

namespace DaimyoDataSolutions.Application.ResultModels
{
    public interface IServiceResult
    {
        bool IsSuccess { get; set; }
        List<string>? ErrorMessages { get; set; }
    }

    public class ServiceResult<T> : IServiceResult
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public List<string>? ErrorMessages { get; set; }
    }

    public class ServiceResult : IServiceResult
    {
        public bool IsSuccess { get; set; }
        public List<string>? ErrorMessages { get; set; }
    }

    public static class ServiceResultExtensions
    {
        public static bool IsRecordNotFound(this IServiceResult serviceResult)
        {
            return serviceResult.ErrorMessages?.Contains(ServiceConstants.RecordNotFound) == true;
        }
    }
}
