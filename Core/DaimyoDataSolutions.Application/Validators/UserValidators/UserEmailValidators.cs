using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.UserValidators
{
    public class UserEmailValidator : BaseValidator<User>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(User value)
        {
            var isSuccess = string.IsNullOrEmpty(value.Email) || value.Email.Length <= 100;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Email exceeds 50 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(User value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
