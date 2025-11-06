using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands;

public record LoginCommand(AuthRequest Request) : IRequest<Result<AuthResponse>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}

public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Request.Email);
        if (user == null)
            return Result<AuthResponse>.Fail("Credenciales inválidas");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, command.Request.Password);
        if (!isPasswordValid)
            return Result<AuthResponse>.Fail("Credenciales inválidas");

        var token = await _tokenService.GenerateTokenAsync(user);

        return Result<AuthResponse>.OK(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            UserName = user.UserName!,
            Authenticated = true
        });
    }
}
