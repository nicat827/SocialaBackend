using Microsoft.Extensions.Configuration;
using SocialaBackend.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocialaBackend.Domain.Entities;

namespace SocialaBackend.Infrastructure.Implementations
{
    internal class EmailService : IEmailService
    {
        
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string receiver, string body, string subject, bool isHtml = true)
        {
            SmtpClient smtpClient = new SmtpClient(_configuration["ApplicationEmail:Host"],
                                                   Convert.ToInt32(_configuration["ApplicationEmail:Port"]));
            smtpClient.EnableSsl = false;

            smtpClient.Credentials = new NetworkCredential(_configuration["ApplicationEmail:Email"],
                                                           _configuration["ApplicationEmail:Password"]);
            MailAddress from = new MailAddress(_configuration["ApplicationEmail:Email"], "Socialite");
            MailAddress to = new MailAddress(receiver);

            MailMessage mail = new MailMessage(from, to);

            mail.Subject = subject;
            mail.IsBodyHtml = isHtml;
            mail.Body = body;

            await smtpClient.SendMailAsync(mail);

        }



    }
}
