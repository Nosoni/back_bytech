using System;
using Application.Common;
using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest dto);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest dto);
    }
}
