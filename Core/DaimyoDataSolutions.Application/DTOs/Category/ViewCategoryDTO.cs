namespace DaimyoDataSolutions.Application.DTOs.Category
{
    public class ViewCategoryDTO : BaseCategoryDTO
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
