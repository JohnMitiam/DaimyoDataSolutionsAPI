using DaimyoDataSolutions.Application.DTOs.Affiliate;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DaimyoDataSolutions.API.Controllers
{
    [Route("api/affiliate")]
    [ApiController]
    [Authorize]
    public class AffiliateController : ControllerBase
    {
        private readonly IAffiliateService _affiliate;
        private IServiceResult ServiceResult { get; set; } = null!;
        public AffiliateController(IAffiliateService affiliate)
        {
            _affiliate = affiliate;
        }

        private string? UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetAffiliatesAsync([FromQuery] AffiliateResourceParameters resourceParameters)
        {
            ServiceResult = await _affiliate.GetAsync(resourceParameters);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<PaginatedList<ViewAffiliateDTO>>;

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

        [HttpGet("{id}", Name = nameof(AffiliateController.GetAffiliateByIdAsync))]
        public async Task<IActionResult> GetAffiliateByIdAsync(int id)
        {
            ServiceResult = await _affiliate.GetByIdAsync(id);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewAffiliateDTO>;
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
        public async Task<IActionResult> CreateAffiliateAsync([FromBody] CreateAffiliateDTO affiliate)
        {
            if (UserId is null)
                return Unauthorized();

            ServiceResult = await _affiliate.CreateAsync(affiliate, UserId);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewAffiliateDTO>;
                if (result != null)
                {
                    return CreatedAtRoute(nameof(AffiliateController.GetAffiliateByIdAsync), new { id = result.Data.Id }, result);
                }
            }

            return BadRequest(ServiceResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAffiliateAsync(int id, [FromBody] UpdateAffiliateDTO affiliate)
        {
            if (UserId is null)
                return Unauthorized();

            if (id == 0 || id != affiliate.Id)
            {
                return BadRequest();
            }

            ServiceResult = await _affiliate.UpdateAsync(id, affiliate, UserId);

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
        public async Task<IActionResult> DeleteAffiliateAsync([FromRoute] int id)
        {
            if (UserId is null)
                return Unauthorized();

            if (id == 0)
            {
                return BadRequest();
            }

            ServiceResult = await _affiliate.DeleteAsync(id, UserId);

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