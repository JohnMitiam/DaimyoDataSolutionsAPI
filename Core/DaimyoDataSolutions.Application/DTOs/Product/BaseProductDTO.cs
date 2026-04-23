namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class BaseProductDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        //public List<int>? ProductCategoryIds { get; set; } = new();
    }
}
