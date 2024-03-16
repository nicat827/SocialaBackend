using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Group:BaseEntity
    {
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageSendedAt { get; set; }
        public string? LastMessageSendedBy { get; set; } = null!;
        public bool LastMessageIsChecked { get; set; }
        public string ConnectionId { get; set; } = null!;
        //relational
        public IList<GroupMessage> Messages { get; set; } = new List<GroupMessage>();
        public ICollection<GroupMemberItem> Members { get; set; } = new List<GroupMemberItem>();

    }
}
