using DaimyoDataSolutions.Application.DTOs.Category;

namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class ViewProductDTO : BaseProductDTO
    {
        public int Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public List<ViewProductCategoryDTO> ProductCategories { get; set; } = new();
    }
}
