using System;

namespace Application.DTOs.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required bool Authenticated { get; set; }
}
