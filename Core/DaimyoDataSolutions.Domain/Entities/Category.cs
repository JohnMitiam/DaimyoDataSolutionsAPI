using DaimyoDataSolutions.Domain.Entities.Base;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class Category : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
