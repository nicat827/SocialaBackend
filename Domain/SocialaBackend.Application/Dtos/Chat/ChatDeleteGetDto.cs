using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class ChatDeleteGetDto
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; } = null!;
        public string ChatPartnerUserName { get; set; } = null!;
        public MessageGetDto? CurrentLastMessage { get; set; } = new MessageGetDto();
        public bool IsDeletedMessageChecked { get; set; }
        public int DeletedMessageId { get; set; }

    }
}
