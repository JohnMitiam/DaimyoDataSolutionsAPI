using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.CategoryValidators
{
    public class CategoryValidator : ICategoryValidator
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Category value)
        {
            return await Task.FromResult(IsValid(value));
        }

        public (bool isSuccess, List<string>? errorMessages) IsValid(Category value)
        {
            var descriptionLengthValidator = new DescriptionLenghtValidator();
            var nameLengthValidator = new NameLengthValidator();

            var categoryValidator = descriptionLengthValidator
                .And(nameLengthValidator);

            var result = categoryValidator.IsValid(value);
            return (result.isSuccess, result.errorMessages);
        }
    }
}
