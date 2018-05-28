using System.Collections.Generic;
using System.Threading.Tasks;

namespace Itsomax.Module.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> MailgunSendEmail(string email, string subject, string message, string domain,
            string apiKey, string emailSender);

        Task SmtpSendEmail(IEnumerable<string> email, string subject, string message, string smtpServer,
            string emailSender, string user, string password, bool enableSsl, int port, string replyAddress);
    }
}