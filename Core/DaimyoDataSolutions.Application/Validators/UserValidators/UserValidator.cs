using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.UserValidators
{
    public class UserValidator : IUserValidator
    {
        private readonly IUserRepository _userRepository;

        public UserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(User value)
        {
            return await Task.FromResult(IsValid(value));
        }

        public (bool isSuccess, List<string>? errorMessages) IsValid(User value)
        {
            var nameLengthValidator = new UserNameLenghtValidator();
            var emailLengthValidator = new UserEmailValidator();

            var userValidator = nameLengthValidator
                .And(emailLengthValidator);

            var result = userValidator.IsValid(value);
            return (result.isSuccess, result.errorMessages);
        }

        //public Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(User user)
        //{
        //    var canDeleteValidator = new UserCanDeleteValidators(_userRepository);
        //    return canDeleteValidator.IsValidAsync(user);
        //}
    }
}
