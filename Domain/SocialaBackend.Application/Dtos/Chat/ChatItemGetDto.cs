using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class ChatItemGetDto
    {
        public int ChatId { get; set; }
        public string ChatPartnerUserName { get; set; } = null!;
        public string? ChatPartnerImageUrl { get; set; }
        public string? LastMessage { get; set; }
        public bool LastMessageIsChecked { get; set; }
        public DateTime? LastMessageSendedAt { get; set; }
        public string LastMessageSendedBy { get; set; } = null!;


    }
}
