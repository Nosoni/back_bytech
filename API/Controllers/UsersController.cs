using Application.DTOs.Users;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Solo administradores pueden gestionar usuarios
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRequest request)
        {
            var result = await _mediator.Send(new CreateUserCommand(request));

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserRequest request)
        {
            var result = await _mediator.Send(new UpdateUserCommand(id, request));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
