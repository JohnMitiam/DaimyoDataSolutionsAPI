using DaimyoDataSolutions.API.Authentications;
using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using Microsoft.AspNetCore.Mvc;

namespace DaimyoDataSolutions.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _product;
        private IServiceResult ServiceResult { get; set; } = null!;
        public ProductController(IProductService product)
        {
            _product = product;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync([FromQuery] ProductResourceParameters resourceParameters)
        {
            ServiceResult = await _product.GetAsync(resourceParameters);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<PaginatedList<ViewProductDTO>>;

                if (result != null)
                {
                    return Ok(new
                    {
                        data = result.Data,
                        total = result.Data.TotalCount,
                        page = result.Data.Page,
                        pageSize = result.Data.PageSize,
                        totalPages = result.Data.TotalPages,
                    });
                }
            }

            return BadRequest(ServiceResult);
        }

        [HttpGet("{id}", Name = nameof(ProductController.GetProductByIdAsync))]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            ServiceResult = await _product.GetByIdAsync(id);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewProductDTO>;
                if (result != null)
                {
                    return Ok(result);
                }
            }

            if (ServiceResult.IsRecordNotFound())
            {
                return NotFound();
            }

            return BadRequest(ServiceResult);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDTO product)
        {
            //var userId = User.GetUserId();

            ServiceResult = await _product.CreateAsync(product);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewProductDTO>;
                if (result != null)
                {
                    return CreatedAtRoute(nameof(ProductController.GetProductByIdAsync), new { id = result.Data.Id }, result);
                }
            }

            return BadRequest(ServiceResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] UpdateProductDTO product)
        {

            //var userId = User.GetUserId();

            if (id == 0 || id != product.Id)
            {
                return BadRequest();
            }

            ServiceResult = await _product.UpdateAsync(id, product);

            if (ServiceResult.IsSuccess)
            {
                return NoContent();
            }

            if (ServiceResult.IsRecordNotFound())
            {
                return NotFound();
            }

            return BadRequest(ServiceResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id)
        {
            //var userId = User.GetUserId();

            if (id == 0)
            {
                return BadRequest();
            }

            ServiceResult = await _product.DeleteAsync(id);

            if (ServiceResult.IsSuccess)
            {
                return NoContent();
            }

            if (ServiceResult.IsRecordNotFound())
            {
                return NotFound();
            }

            return BadRequest(ServiceResult);
        }

    }
}