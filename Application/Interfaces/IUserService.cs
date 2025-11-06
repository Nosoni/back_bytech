using System;
using Application.Common;
using Application.DTOs.Users;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponse>> CreateUserAsync(UserRequest dto);
        Task<Result<UserResponse>> UpdateUserAsync(string userId, UserRequest dto);
        Task<Result<UserResponse>> GetUserByIdAsync(string userId);
        Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync();
        Task<Result<bool>> DeleteUserAsync(string userId);
    }
}
