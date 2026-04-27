using DaimyoDataSolutions.Application.DTOs.Category;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DaimyoDataSolutions.API.Controllers
{
    [Route("api/category")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _category;
        private IServiceResult ServiceResult { get; set; } = null!;
        public CategoryController(ICategoryService category)
        {
            _category = category;
        }
        private string? UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        [AllowAnonymous]
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

        [HttpGet("{id}", Name = nameof(GetCategoryByIdAsync))]
        [AllowAnonymous]
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
            if (UserId is null)
                return Unauthorized();

            //var result = await _category.CreateAsync(category, UserId);

            ServiceResult = await _category.CreateAsync(category, UserId);

            if (ServiceResult.IsSuccess)
            {
                var serviceResult = ServiceResult as ServiceResult<ViewCategoryDTO>;
                if (serviceResult != null)
                {
                    return CreatedAtRoute(nameof(CategoryController.GetCategoryByIdAsync), new { id = serviceResult.Data.Id }, ServiceResult);
                }
            }

            return BadRequest(ServiceResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryDTO category)
        {

            if (UserId is null)
                return Unauthorized();

            if (id == 0 || id != category.Id)
            {
                return BadRequest();
            }

            ServiceResult = await _category.UpdateAsync(id, category, UserId);

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
            if (UserId is null)
                return Unauthorized();

            //var affiliateId = User.GetUserId();

            if (id == 0)
            {
                return BadRequest();
            }

            ServiceResult = await _category.DeleteAsync(id, UserId);

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