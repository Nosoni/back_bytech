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

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result<AuthResponse>.Fail("Email ya registrado");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<AuthResponse>.Fail(
                    "Error al crear usuario",
                    result.Errors.Select(e => e.Description).ToList()
                );

            var token = await _tokenService.GenerateTokenAsync(user);

            return Result<AuthResponse>.OK(new AuthResponse
            {
                Token = token,
                Email = user.Email!,
                UserName = user.UserName!,
                Authenticated = true
            });
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest dto)
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
