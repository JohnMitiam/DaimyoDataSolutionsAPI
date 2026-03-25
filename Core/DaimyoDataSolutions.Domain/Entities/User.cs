using DaimyoDataSolutions.Domain.Entities.Base;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class User : BaseModel
    {
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
