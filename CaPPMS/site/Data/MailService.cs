using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CaPPMS.Data
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailConfig;

        public MailService(MailSettings mailConfig)
        {
            _mailConfig = mailConfig;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_mailConfig.FromEmail),
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlBody
            };
            message.To.Add(new MailAddress(toEmail));

            using var smtp = new SmtpClient
            {
                Port = _mailConfig.Port,
                Host = _mailConfig.Host,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailConfig.Username, _mailConfig.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtp.SendMailAsync(message);
        }
    }
}
