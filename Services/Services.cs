using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Gym_Membership.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = _config["EmailSettings:FromEmail"];
            var appPassword = _config["EmailSettings:AppPassword"];
            var host = _config["EmailSettings:Host"];
            var port = int.Parse(_config["EmailSettings:Port"]);

            using (var smtp = new SmtpClient(host, port))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromEmail, appPassword);

                var message = new MailMessage(fromEmail, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(message);
            }
        }
    }
}
