using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiver, string body, string subject, bool isHtml = true);
    }
}
