using Microsoft.AspNetCore.Mvc;
using DaimyoDataSolutions.API.Authentications;
using DaimyoDataSolutions.Application.DTOs.User;
using DaimyoDataSolutions.Application.Interfaces.Services;
using DaimyoDataSolutions.Application.ResourceParameters;
using DaimyoDataSolutions.Application.ResultModels;

namespace DaimyoDataSolutions.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        private IServiceResult ServiceResult { get; set; } = null!;
        public UserController(IUserService user)
        {
            _user = user;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UserResourceParameters resourceParameters)
        {
            ServiceResult = await _user.GetAsync(resourceParameters);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<PaginatedList<ViewUserDTO>>;

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

        [HttpGet("{id}", Name = nameof(UserController.GetUserByIdAsync))]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            ServiceResult = await _user.GetByIdAsync(id);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewUserDTO>;
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
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDTO user)
        {
            var userId = User.GetUserId();

            ServiceResult = await _user.CreateAsync(user, userId);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewUserDTO>;
                if (result != null)
                {
                    return CreatedAtRoute(nameof(UserController.GetUserByIdAsync), new { id = result.Data.Id }, result);
                }
            }

            return BadRequest(ServiceResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDTO user)
        {

            var userId = User.GetUserId();

            if (id == 0 || id != user.Id)
            {
                return BadRequest();
            }

            ServiceResult = await _user.UpdateAsync(id, user, userId);

            if (ServiceResult.IsSuccess)
            {
                var result = ServiceResult as ServiceResult<ViewUserDTO>;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            var userId = User.GetUserId();

            if (id == 0)
            {
                return BadRequest();
            }

            ServiceResult = await _user.DeleteAsync(id, userId);

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