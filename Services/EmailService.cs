
using Microsoft.Extensions.Options;
using Rezk_Proj.Helpers;
using System.Net;
using System.Net.Mail;

namespace Rezk_Proj.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string mailTo, string subject, string body)
        {

            using (var client = new SmtpClient())
            {
                client.Host = _mailSettings.SmtpServer;   
                client.Port = _mailSettings.Port;        
                client.EnableSsl = true;                  
                client.Credentials = new NetworkCredential(
                    _mailSettings.Username,
                    _mailSettings.Password
                );

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(_mailSettings.From, _mailSettings.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true 
                };

                mailMessage.To.Add(mailTo);

                await client.SendMailAsync(mailMessage);
               
            
            }
        }
    }
}
