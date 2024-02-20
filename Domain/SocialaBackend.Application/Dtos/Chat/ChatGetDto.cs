using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos.Chat
{
    public class ChatGetDto
    {
        public int Id { get; set; }
        public string ChatPartnerUserName { get; set; } = null!;
        public string ChatPartnerName { get; set; } = null!;
        public string ChatPartnerSurname { get; set; } = null!;
        public string? ChatPartnerImageUrl { get; set; }
        public string ConnectionId { get; set; } = null!;
        public IEnumerable<MessageGetDto> Messages { get; set; } = new List<MessageGetDto>();
    }
}
