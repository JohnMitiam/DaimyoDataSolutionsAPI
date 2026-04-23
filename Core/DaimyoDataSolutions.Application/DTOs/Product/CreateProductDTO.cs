namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class CreateProductDTO : BaseProductDTO
    {
        public List<int> ProductCategoryIds { get; set; } = new();
    }
}
