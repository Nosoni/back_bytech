using Application.Features.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Authorization;
using static API.Authorization.Permissions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [RequirePermission(PermissionsManagement.Read)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await _mediator.Send(new GetAllPermissionsQuery());

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
