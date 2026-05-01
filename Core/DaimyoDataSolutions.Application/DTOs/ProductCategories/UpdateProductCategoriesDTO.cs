using DaimyoDataSolutions.Application.DTOs.ProductCategories;

namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class UpdateProductCategoriesDTO: BaseProductCategoriesDTO
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
