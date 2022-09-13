using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_Lib.DI;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CMS.Services.Emails;

public interface IEmailService : IScoped
{
    public void SendEmailAsync(Message message);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _iLogger;
    private EmailConfiguration _emailConfig;

    public EmailService(ILogger<EmailService> iLogger, IConfiguration iConfiguration)
    {
        _iLogger = iLogger;
        _emailConfig = new EmailConfiguration()
        {
            FromEmail = iConfiguration.GetSection("AppSetting:Email:Email").Value,
            SmtpServer = iConfiguration.GetSection("AppSetting:Email:SmtpServer").Value,
            Port = iConfiguration.GetSection("AppSetting:Email:Port").Get<int>(),
            UserName = iConfiguration.GetSection("AppSetting:Email:UserName").Value,
            Password = iConfiguration.GetSection("AppSetting:Email:Password").Value,
        };
    }

    public void SendEmailAsync(Message message)
    {
        var mailMessage = CreateEmailMessage(message);
         SendAsync(mailMessage, message);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Hệ thống Prugift.vn", _emailConfig.FromEmail));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        var bodyBuilder = new BodyBuilder { HtmlBody = message.Content };
        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private void SendAsync(MimeMessage mailMessage, Message message)
    {
        using var client = new SmtpClient();
        try
        {
            client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            // client.AuthenticationMechanisms.Remove("XOAUTH");
            client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
            client.Send(mailMessage);
            _iLogger.LogInformation($"Gửi email đến {message.To} thành công: {mailMessage.Subject}");
        }
        catch(Exception ex)
        {
            this._iLogger.LogError(ex,"Send email err");
            //log an error message or throw an exception, or both.
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
    }
}

public class Message
{
    public List<MailboxAddress> To { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public Message(List<string> to, string subject, string content)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress("Prugift Email", x)));
        Subject = subject;
        Content = content;
    }
}

public class EmailConfiguration
{
    public string FromEmail { get; set; }

    public string SmtpServer { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}