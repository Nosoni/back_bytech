using System;
using Application.Common;
using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(AuthRequest dto);
    }
}
