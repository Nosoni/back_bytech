using Application.DTOs.Users;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Authorization;
using static API.Authorization.Permissions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [RequirePermission(Users.Create)]
        public async Task<IActionResult> CreateUser(UserRequest request)
        {
            var result = await _mediator.Send(new CreateUserCommand(request));

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result);
        }

        [HttpGet("{id}")]
        [RequirePermission(Users.Read)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        [RequirePermission(Users.Read)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [RequirePermission(Users.Update)]
        public async Task<IActionResult> UpdateUser(string id, UserRequest request)
        {
            var result = await _mediator.Send(new UpdateUserCommand(id, request));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [RequirePermission(Users.Delete)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
