using DaimyoDataSolutions.Application.DTOs.ProductCategories;
using DaimyoDataSolutions.Application.DTOs.ProductImages;

namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class CreateProductDTO : BaseProductDTO
    {
        public List<CreateProductCategoriesDTO>? Categories { get; set; }
        public List<CreateProductImagesDTO>? Images { get; set; }
    }
}
