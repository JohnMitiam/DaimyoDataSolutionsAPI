namespace DaimyoDataSolutions.Application.Interfaces.Validator
{
    public interface IValidate<T> where T : class
    {
        Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(T value);
        (bool isSuccess, List<string>? errorMessages) IsValid(T value);
    }
}
