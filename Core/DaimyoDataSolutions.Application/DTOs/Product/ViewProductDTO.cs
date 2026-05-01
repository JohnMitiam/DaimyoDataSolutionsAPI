using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.DTOs.ProductCategories;

namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class ViewProductDTO : BaseProductDTO
    {
        public int Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public List<ViewProductCategoriesDTO> ProductCategories { get; set; } = new();
    }
}
