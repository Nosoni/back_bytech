using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
