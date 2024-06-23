namespace LW.Contracts.Services;

public interface IEmailService<in T> where T : class
{
    Task SendEmailAsync(T request, CancellationToken cancellationToken = new CancellationToken());
}