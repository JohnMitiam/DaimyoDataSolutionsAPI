using DaimyoDataSolutions.Domain.Entities.Base;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class Products : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<ProductCategories> ProductCategories { get; set; } = new List<ProductCategories>();
    }
}
