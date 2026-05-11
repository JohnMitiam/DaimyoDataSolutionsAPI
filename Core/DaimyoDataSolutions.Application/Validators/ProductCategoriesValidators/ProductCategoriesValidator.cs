using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.ProductCategoriesValidators
{
    public class ProductCategoriesValidator : IProductCategoriesValidator
    {
        private readonly IProductCategoriesRepository _productCategoriesRepository;

        public ProductCategoriesValidator(IProductCategoriesRepository productCategoriesRepository)
        {
            _productCategoriesRepository = productCategoriesRepository;
        }

        public (bool isSuccess, List<string>? errorMessages) IsValid(ProductCategories value)
        {
            var errors = new List<string>();

            if (value.CategoryId <= 0)
            {
                errors.Add("A valid Category ID is required.");
            }

            return (errors.Count == 0, errors.Count > 0 ? errors : null);
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(ProductCategories value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}