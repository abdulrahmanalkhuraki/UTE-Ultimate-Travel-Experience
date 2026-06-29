namespace Application.Interfaces.User
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? Name { get; }
        bool IsAuthenticated { get; }
    }
}
