using Application.DTOs.User.Request;
using Application.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User
{
    public interface IUserService
    {
        Task<UserResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<UserResponse> UpdateAsync(int userId, UserUpdateRequest request, CancellationToken cancellationToken);
        Task<UserResponse> UpdateLocationAsync(int userId, UpdateLocationRequest request, CancellationToken cancellationToken);
        Task<UserResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken);
        Task<CompleteProfileResponse> CompleteProfileAsync(int userId, CompleteProfileRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteMyAccountAsync(int userId, DeleteAccountRequest request, CancellationToken cancellationToken);
        Task<bool> AdminDeleteUserAsync(int userId, CancellationToken cancellationToken, bool IsHardDelete = false);

        Task<DeletedUsersResponse> GetDeletedUsersAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<UserResponse>> FilterAsync(
            string? firstName = null,
            string? lastName = null,
            string? email = null,
            string? roleName = null,
            bool? isEmailVerified = null,
            CancellationToken cancellationToken = default);
    }
}
