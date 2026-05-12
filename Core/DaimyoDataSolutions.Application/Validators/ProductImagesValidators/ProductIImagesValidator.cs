using DaimyoDataSolutions.Application.Interfaces.Data;
using DaimyoDataSolutions.Application.Interfaces.Validator;
using DaimyoDataSolutions.Domain.Entities;

namespace DaimyoDataSolutions.Application.Validators.ProductImagesValidators
{
    public class ProductImagesValidator : IProductImagesValidator
    {
        private readonly IProductImagesRepository _productImagesRepository;

        public ProductImagesValidator(IProductImagesRepository productImagesRepository)
        {
            _productImagesRepository = productImagesRepository;
        }

        public (bool isSuccess, List<string>? errorMessages) IsValid(ProductImages value)
        {
            var errors = new List<string>();

            return (errors.Count == 0, errors.Count > 0 ? errors : null);
        }

        public async Task<(bool isSuccess, List<string>? errorMessages)> IsValidAsync(ProductImages value)
        {
            return await Task.FromResult(IsValid(value));
        }
    }
}