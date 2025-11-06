using System;
using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<AuthResponse>> LoginAsync(AuthRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<AuthResponse>.Fail("Credenciales inválidas");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
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
}
