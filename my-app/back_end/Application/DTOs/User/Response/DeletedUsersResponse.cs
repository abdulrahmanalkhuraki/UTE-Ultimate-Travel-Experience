namespace Application.DTOs.User.Response
{
    public sealed class DeletedUsersResponse
    {
        public int TotalCount { get; set; }
        public IReadOnlyList<UserResponse> Users { get; set; } = Array.Empty<UserResponse>();
    }
}
