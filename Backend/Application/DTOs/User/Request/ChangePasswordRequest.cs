namespace Application.DTOs.User.Request
{
    public sealed record ChangePasswordRequest
    (
        string CurrentPassword,
        string NewPassword
    );
}
