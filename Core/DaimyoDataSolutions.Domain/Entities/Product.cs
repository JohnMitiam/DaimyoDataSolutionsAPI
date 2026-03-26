using DaimyoDataSolutions.Domain.Entities.Base;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class Product : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
    }
}
