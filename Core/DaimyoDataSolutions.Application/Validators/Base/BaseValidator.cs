using DaimyoDataSolutions.Application.Interfaces.Validator;

namespace DaimyoDataSolutions.Application.Validators.Base
{
    public abstract class BaseValidator<T> : IValidate<T> where T : class
    {
        public abstract (bool isSuccess, List<string>? errorMessages) IsValid(T value);

        public abstract Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(T value);

        public BaseValidator<T> And(BaseValidator<T> validator)
        {
            return new AndValidator<T>(this, validator);
        }

        public BaseValidator<T> Or(BaseValidator<T> validator)
        {
            return new OrValidator<T>(this, validator);
        }

        public BaseValidator<T> Not()
        {
            return new NotValidator<T>(this);
        }
    }

    public class AndValidator<T> : BaseValidator<T> where T : class
    {
        private readonly BaseValidator<T> _leftValidator;
        private readonly BaseValidator<T> _rightValidator;
        public AndValidator(BaseValidator<T> leftValidator, BaseValidator<T> rightValidator)
        {
            _leftValidator = leftValidator;
            _rightValidator = rightValidator;
        }
        public override (bool isSuccess, List<string>? errorMessages) IsValid(T value)
        {
            var leftResult = _leftValidator.IsValid(value);
            var rightResult = _rightValidator.IsValid(value);

            var isSuccess = leftResult.isSuccess && rightResult.isSuccess;
            var messages = new List<string>();

            if (!leftResult.isSuccess && leftResult.errorMessages != null && leftResult.errorMessages.Any())
            {
                messages.AddRange(leftResult.errorMessages);
            }

            if (!rightResult.isSuccess && rightResult.errorMessages != null && rightResult.errorMessages.Any())
            {
                messages.AddRange(rightResult.errorMessages);
            }

            return (isSuccess, messages);
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(T value)
        {
            var leftResult = await _leftValidator.IsValidAsync(value);
            var rightResult = await _rightValidator.IsValidAsync(value);

            var isSuccess = leftResult.isSuccess && rightResult.isSuccess;
            var messages = new List<string>();

            if (!leftResult.isSuccess && leftResult.errorMessages != null && leftResult.errorMessages.Any())
            {
                messages.AddRange(leftResult.errorMessages);
            }

            if (!rightResult.isSuccess && rightResult.errorMessages != null && rightResult.errorMessages.Any())
            {
                messages.AddRange(rightResult.errorMessages);
            }

            return (isSuccess, messages);
        }
    }

    public class OrValidator<T> : BaseValidator<T> where T : class
    {
        private readonly BaseValidator<T> _leftValidator;
        private readonly BaseValidator<T> _rightValidator;
        public OrValidator(BaseValidator<T> leftValidator, BaseValidator<T> rightValidator)
        {
            _leftValidator = leftValidator;
            _rightValidator = rightValidator;
        }
        public override (bool isSuccess, List<string>? errorMessages) IsValid(T value)
        {
            var leftResult = _leftValidator.IsValid(value);
            var rightResult = _rightValidator.IsValid(value);

            var isSuccess = leftResult.isSuccess || rightResult.isSuccess;
            var messages = new List<string>();

            if (!isSuccess && leftResult.errorMessages != null && leftResult.errorMessages.Any())
            {
                messages.AddRange(leftResult.errorMessages);
            }

            if (!isSuccess && rightResult.errorMessages != null && rightResult.errorMessages.Any())
            {
                messages.AddRange(rightResult.errorMessages);
            }

            return (isSuccess, messages);
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(T value)
        {
            var leftResult = await _leftValidator.IsValidAsync(value);
            var rightResult = await _rightValidator.IsValidAsync(value);

            var isSuccess = leftResult.isSuccess || rightResult.isSuccess;
            var messages = new List<string>();

            if (!isSuccess && leftResult.errorMessages != null && leftResult.errorMessages.Any())
            {
                messages.AddRange(leftResult.errorMessages);
            }

            if (!isSuccess && rightResult.errorMessages != null && rightResult.errorMessages.Any())
            {
                messages.AddRange(rightResult.errorMessages);
            }

            return (isSuccess, messages);
        }
    }

    public class NotValidator<T> : BaseValidator<T> where T : class
    {
        private readonly BaseValidator<T> _validator;
        public NotValidator(BaseValidator<T> validator)
        {
            _validator = validator;
        }
        public override (bool isSuccess, List<string>? errorMessages) IsValid(T value)
        {
            var result = _validator.IsValid(value);

            var isSuccess = !result.isSuccess;
            var messages = new List<string>();

            if (!isSuccess && result.errorMessages != null && result.errorMessages.Any())
            {
                messages.AddRange(result.errorMessages);
            }

            return (isSuccess, messages);
        }

        public override async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(T value)
        {
            var result = await _validator.IsValidAsync(value);

            var isSuccess = !result.isSuccess;
            var messages = new List<string>();

            if (!isSuccess && result.errorMessages != null && result.errorMessages.Any())
            {
                messages.AddRange(result.errorMessages);
            }

            return (isSuccess, messages);
        }
    }
}
