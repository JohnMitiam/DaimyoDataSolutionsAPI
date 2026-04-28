using DaimyoDataSolutions.Application.DTOs.Product;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DaimyoDataSolutions.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _product;
        private IServiceResult ServiceResult { get; set; } = null!;
        public ProductController(IProductService product)
        {
            _product = product;
        }
        private string? UserId =>
           User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
            if (UserId is null)
                return Unauthorized();

            ServiceResult = await _product.CreateAsync(product, UserId);

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
            if (UserId is null)
                return Unauthorized();

            if (id == 0 || id != product.Id)
            {
                return BadRequest();
            }

            ServiceResult = await _product.UpdateAsync(id, product, UserId);

            if (!ServiceResult.IsSuccess)
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
            if (UserId is null)
                return Unauthorized();

            if (id == 0)
            {
                return BadRequest();
            }

            ServiceResult = await _product.DeleteAsync(id, UserId);

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

        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProductsAsync([FromQuery] ProductResourceParameters resourceParameters)
        {
            if (UserId is null)
                return Unauthorized();

            ServiceResult = await _product.GetMyProductsAsync(UserId);
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
            return Ok(ServiceResult);
        }

    }
}