namespace DaimyoDataSolutions.Application.DTOs.Affiliate
{
    public class BaseAffiliateDTO
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
