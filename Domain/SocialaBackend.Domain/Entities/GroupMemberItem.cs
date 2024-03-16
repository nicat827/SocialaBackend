using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class GroupMemberItem:BaseEntity
    {
        public GroupRole Role { get; set; }
        //relational
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
    }
}
