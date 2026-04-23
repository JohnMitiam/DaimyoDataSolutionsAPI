using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.ProductValidators
{
    public class ProductValidator : IProductValidator
    {
        private readonly IProductRepository _productRepository;

        public ProductValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(Products value)
        {
            return await Task.FromResult(IsValid(value));
        }

        public (bool isSuccess, List<string>? errorMessages) IsValid(Products value)
        {
            var nameLengthValidator = new NameLengthValidator();

            var productValidator = nameLengthValidator;

            var result = productValidator.IsValid(value);
            return (result.isSuccess, result.errorMessages);
        }

        //public Task<(bool isSuccess, List<string>? errorMessages)> IsValidForDeleteAsync(User affiliate)
        //{
        //    var canDeleteValidator = new UserCanDeleteValidators(_affiliateRepository);
        //    return canDeleteValidator.IsValidAsync(affiliate);
        //}
    }
}
