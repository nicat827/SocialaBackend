using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos.Chat.Message
{
    public class AudioMessagePostDto
    {
        public IFormFile Audio { get; set; } = null!;

        public int ChatId { get; set; }

        public int Minutes { get; set; }

        public int Seconds { get; set; }
        public string Sender { get; set; } = null!;
    }
}
