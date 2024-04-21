using Application.Common.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUsername = "magazinehub1640@gmail.com";
    private readonly string _smtpPassword = "obkh hpiv kvlz dcnn";
    private readonly SmtpClient _client;


    public EmailService()
    {
        _client = new SmtpClient
        {
            Host = _smtpServer,
            Port = _smtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
        };
    }


    public Task SendEmailAsync(string toEmailAddress, string subject, string message)
    {
        var mail = new MailMessage(from: _smtpUsername, to: toEmailAddress)
        {
            Subject = subject,
            Body = $"{message}",
            IsBodyHtml = true,

        };
        return _client.SendMailAsync(mail);
    }
}