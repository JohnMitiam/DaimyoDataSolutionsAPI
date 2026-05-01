using DaimyoDataSolutions.Application.DTOs.Category;

namespace DaimyoDataSolutions.Application.DTOs.ProductCategories
{
    public class ViewProductCategoriesDTO : BaseProductCategoriesDTO
    {
        public int Id {  get; set; }
        public string? PropertyId {  get; set; }
        public string? CategoryName { get; set; }
    }
}
