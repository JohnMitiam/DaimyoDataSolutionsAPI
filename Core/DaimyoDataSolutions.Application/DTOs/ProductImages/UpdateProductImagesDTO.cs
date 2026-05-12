namespace DaimyoDataSolutions.Application.DTOs.ProductImages
{
    public class UpdateProductImagesDTO: BaseProductImagesDTO
    {
        public int Id { get; set; }
        public byte[]? ImageData { get; set; }
        public string? MimeType { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }
    }
}
