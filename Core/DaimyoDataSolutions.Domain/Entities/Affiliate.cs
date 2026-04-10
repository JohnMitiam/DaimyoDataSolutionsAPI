using DaimyoDataSolutions.Domain.Entities.Base;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class Affiliate : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
