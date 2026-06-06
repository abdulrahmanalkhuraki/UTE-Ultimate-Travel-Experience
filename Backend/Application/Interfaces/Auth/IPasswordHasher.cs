<<<<<<<< HEAD:Backend/Application/Interfaces/Auth/IPasswordHasher.cs
namespace Application.Interfaces.Auth;
========
namespace Application.Interfaces.User;
>>>>>>>> eb3c5c2000dac9b658f595448513569eb27a78bb:Backend/Application/Interfaces/User/IPasswordHasher.cs

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
