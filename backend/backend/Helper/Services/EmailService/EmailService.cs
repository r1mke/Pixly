using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace backend.Helper.Services.EmailSender
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient
            {
                Host = _config["Smtp:Host"],
                Port = int.Parse(_config["Smtp:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _config["Smtp:Username"],
                    _config["Smtp:Password"]
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Smtp:FromEmail"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            // Logovanje SMTP postavki (za debagovanje)
            Console.WriteLine($"Using SMTP Host: {_config["Smtp:Host"]}, Port: {_config["Smtp:Port"]}");

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
