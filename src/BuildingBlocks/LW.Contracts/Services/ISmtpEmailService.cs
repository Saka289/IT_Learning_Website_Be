using LW.Shared.Services.Email;

namespace LW.Contracts.Services;

public interface ISmtpEmailService : IEmailService<MailRequest>
{
    
}