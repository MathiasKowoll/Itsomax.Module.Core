using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Itsomax.Module.Core.Interfaces;

namespace Itsomax.Module.Core.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> MailgunSendEmail(string email, string subject, string message, string domain,
            string apiKey,string emailSender)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + apiKey)));
            
            var form = new Dictionary<string, string>();
            form["from"] = emailSender;
            form["to"] = email;
            form["subject"] = subject;
            form["text"] = "Email Enviado exitosamente";
            //form["o:require-tls"] = "true";
            form["h:Reply-To"] = email;
            form["html"] = message;

            var response = await client.PostAsync("https://api.mailgun.net/v3/" + domain + "/messages",
                new FormUrlEncodedContent(form));
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return  true;
                default:
                    return false;
            }
        }

        public Task SmtpSendEmail(IEnumerable<string> email, string subject, string message, string smtpServer,
            string emailSender,string user,string password, bool enableSsl,int port,string replyAddress, IEnumerable<string> fileNames)
        {
            //TODO: Implement atachments
            //var contentType = new ContentType();
            //contentType.MediaType = MediaTypeNames.Application.Octet;
            //contentType.Name = fileName;
            var client = new SmtpClient(smtpServer)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, password),
                EnableSsl = enableSsl,
                Port = port
            };

            var mail = new MailMessage()
            {
                From = new MailAddress(emailSender),
                IsBodyHtml = true,
                Body = message,
                Subject = subject
            };
            mail.ReplyToList.Add(replyAddress);
            
            foreach (var emailAddress in email)
            {
                mail.To.Add(emailAddress);
            }

            client.Send(mail);
            mail.Dispose();
            
            return Task.FromResult(0);
            
        }

        public Task SmtpSendEmailAnonymous(IEnumerable<string> email, string subject, string message, string smtpServer,
            string emailSender, bool enableSsl, int port, string replyAddress, IEnumerable<string> fileNames)
        {
            var client = new SmtpClient(smtpServer)
            {
                UseDefaultCredentials = false,
                EnableSsl = enableSsl,
                Port = port
            };

            var mail = new MailMessage()
            {
                From = new MailAddress(emailSender),
                IsBodyHtml = true,
                Body = message,
                Subject = subject
            };
            mail.ReplyToList.Add(replyAddress);
            
            foreach (var emailAddress in email)
            {
                mail.To.Add(emailAddress);
            }

            client.Send(mail);
            mail.Dispose();
            
            return Task.FromResult(0);
        }
    }
    
}