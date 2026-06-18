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
        Task<UserResponse> UpdateMeAsync(int userId, UserUpdateRequest request, CancellationToken cancellationToken);
        Task<UserResponse> CompleteProfileAsync(int userId, CompleteProfileRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteMyAccountAsync(int userId, DeleteAccountRequest request, CancellationToken cancellationToken);
        Task<bool> AdminDeleteUserAsync(int userId, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserResponse>> FilterAsync(
            string? firstName = null,
            string? lastName = null,
            string? email = null,
            int? roleId = null,
            bool? isEmailVerified = null,
            CancellationToken cancellationToken = default);
    }
}
