using System;
namespace DaimyoDataSolutions.Domain.Entities.Base
{
    public class BaseModel
    {
        // Key
        public int Id { get; set; }


        // Audit Fields
        public string CreatedBy { get; set; } = null!;
        public string? UpdatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
