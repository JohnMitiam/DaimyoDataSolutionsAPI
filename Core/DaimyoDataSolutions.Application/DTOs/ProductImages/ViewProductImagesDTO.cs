using DaimyoDataSolutions.Application.DTOs.Category;

namespace DaimyoDataSolutions.Application.DTOs.ProductImages
{
    public class ViewProductImagesDTO : BaseProductImagesDTO
    {
        public byte[]? ImageData { get; set; }
    }
}
