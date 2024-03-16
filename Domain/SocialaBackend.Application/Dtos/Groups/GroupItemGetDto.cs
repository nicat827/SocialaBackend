using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class GroupItemGetDto
    {
        public int GroupId { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? LastMessage { get; set; }
        public bool LastMessageIsChecked { get; set; }
        public DateTime? LastMessageSendedAt { get; set; }
        public string? LastMessageSendedBy { get; set; }
        public int UnreadedMessagesCount { get; set; }


    }
}
