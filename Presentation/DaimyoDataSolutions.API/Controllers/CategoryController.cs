using DaimyoDataSolutions.API.Authentications;
using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using Microsoft.AspNetCore.Mvc;

namespace DaimyoDataSolutions.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _category;
        private IServiceResult ServiceResult { get; set; } = null!;
        public CategoryController(ICategoryService category)
        {
            _category = category;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorysAsync([FromQuery] CategoryResourceParameters resourceParameters)
        {
            ServiceResult = await _category.GetAsync(resourceParameters);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<PaginatedList<ViewCategoryDTO>>;

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

        [HttpGet("{id}", Name = nameof(CategoryController.GetCategoryByIdAsync))]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            ServiceResult = await _category.GetByIdAsync(id);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewCategoryDTO>;
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
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDTO category)
        {
            //var affiliateId = User.GetUserId();

            ServiceResult = await _category.CreateAsync(category);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewCategoryDTO>;
                if (result != null)
                {
                    return CreatedAtRoute(nameof(CategoryController.GetCategoryByIdAsync), new { id = result.Data.Id }, result);
                }
            }

            return BadRequest(ServiceResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryDTO category)
        {

            //var affiliateId = User.GetUserId();

            if (id == 0 || id != category.Id)
            {
                return BadRequest();
            }

            ServiceResult = await _category.UpdateAsync(id, category);

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
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            //var affiliateId = User.GetUserId();

            if (id == 0)
            {
                return BadRequest();
            }

            ServiceResult = await _category.DeleteAsync(id);

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