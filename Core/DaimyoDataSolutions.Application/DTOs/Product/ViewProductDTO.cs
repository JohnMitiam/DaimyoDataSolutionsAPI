namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class ViewProductDTO : BaseProductDTO
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
