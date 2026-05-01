using DaimyoDataSolutions.Application.DTOs.ProductCategories;

namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class CreateProductDTO : BaseProductDTO
    {
        public List<CreateProductCategoriesDTO> Categories { get; set; }
    }
}
