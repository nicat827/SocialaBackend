using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class GroupMessageWatcher : BaseEntity
    {
        public int GroupMessageId { get; set; }
        public GroupMessage GroupMessage { get; set; } = null!;
        public string? AppUserId { get; set; } = null!;
        public AppUser? AppUser { get; set; } = null!;
    }
}
