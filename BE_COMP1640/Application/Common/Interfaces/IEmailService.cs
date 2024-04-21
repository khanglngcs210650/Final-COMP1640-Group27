namespace Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmailAddress, string subject, string message);
}