using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class GroupMessage:BaseEntity
    {
        public string? Text { get; set; } = null!;
        //public ICollection<MessageMedia> Media { get; set; } = new List<MessageMedia>();
        public string Sender { get; set; } = null!;

        public string? ImageUrl { get; set; }
        public ICollection<GroupMessageWatcher> CheckedUsers { get; set; } = new List<GroupMessageWatcher>();
        //relational
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
    }
}
