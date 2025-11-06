using Application.DTOs.Auth;
using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest request)
        {
            var result = await _mediator.Send(new LoginCommand(request));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
