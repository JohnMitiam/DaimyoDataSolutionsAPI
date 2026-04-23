using DaimyoDataSolutions.Application.DTOs.Affiliate;

namespace DaimyoDataSolutions.Application.DTOs.Affiliate
{
    public class ViewAffiliateDTO : BaseAffiliateDTO
    {
        public int Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
