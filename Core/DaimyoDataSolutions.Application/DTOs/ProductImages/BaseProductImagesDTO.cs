namespace DaimyoDataSolutions.Application.DTOs.ProductImages
{
    public class BaseProductImagesDTO
    {
        public byte[] ImageData { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
