namespace Application.Interfaces.User;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
