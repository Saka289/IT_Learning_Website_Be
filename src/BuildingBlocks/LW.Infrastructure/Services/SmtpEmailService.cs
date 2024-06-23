using System.Text;
using LW.Contracts.Services;
using LW.Infrastructure.Configurations;
using LW.Shared.Services.Email;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;

namespace LW.Infrastructure.Services;

public class SmtpEmailService : ISmtpEmailService
{
    private readonly ILogger _logger;
    private readonly SMTPEmailSetting _setting;
    private readonly SmtpClient _smtpClient;

    public SmtpEmailService(ILogger logger, SMTPEmailSetting setting)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _smtpClient = new SmtpClient();
    }

    public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var emailMessage = new MimeMessage()
        {
            Sender = new MailboxAddress(_setting.DisplayName, request.From ?? _setting.From),
            Subject = request.Subject,
            Body = new BodyBuilder
            {
                HtmlBody = request.Body
            }.ToMessageBody()
        };

        if (request.ToAddresses.Any())
        {
            foreach (var toAddress in request.ToAddresses)
            {
                emailMessage.To.Add((MailboxAddress.Parse(toAddress)));
            }
        }
        else
        {
            var toAddress = request.ToAddress;
            emailMessage.To.Add(MailboxAddress.Parse(toAddress));
        }

        try
        {
            await _smtpClient.ConnectAsync(_setting.SMTPServer, _setting.Port, _setting.UseSsl, cancellationToken);
            await _smtpClient.AuthenticateAsync(_setting.UserName, _setting.Password, cancellationToken);
            await _smtpClient.SendAsync(emailMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }
        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
            _smtpClient.Dispose();
        }
    }
}