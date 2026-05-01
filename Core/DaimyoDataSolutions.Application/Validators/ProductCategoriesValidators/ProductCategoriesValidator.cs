using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Application.Validators.CategoryValidators;
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
            throw new NotImplementedException();
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(ProductCategories value)
        {
            return await Task.FromResult(IsValid(value));
        }

        
    }
}