namespace DaimyoDataSolutions.Application.DTOs.Product
{
    public class UpdateProductDTO : BaseProductDTO
    {
        public int Id { get; set; }
        public List<UpdateProductCategoriesDTO> ProductCategories { get; set; }
    }
}
