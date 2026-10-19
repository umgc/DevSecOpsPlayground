using System.Threading.Tasks;

namespace CaPPMS.Data
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
