<<<<<<<< HEAD:Backend/Application/Interfaces/Auth/IEmailSender.cs
namespace Application.Interfaces.Auth;
========
namespace Application.Interfaces.User;
>>>>>>>> eb3c5c2000dac9b658f595448513569eb27a78bb:Backend/Application/Interfaces/User/IEmailSender.cs

public interface IEmailSender
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default);
}
