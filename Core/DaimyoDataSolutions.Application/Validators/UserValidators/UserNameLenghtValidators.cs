using DaimyoDataSolutions.Application.Validators.Base;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.UserValidators
{
    public class UserNameLenghtValidator : BaseValidator<User>
    {
        public override (bool isSuccess, List<string>? errorMessages) IsValid(User value)
        {
            var isSuccess = string.IsNullOrEmpty(value.UserName) || value.UserName.Length <= 50;
            if (isSuccess)
            {
                return (true, null);
            }

            return (false, new List<string> { "Name exceeds 100 characters." });
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(User value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}
