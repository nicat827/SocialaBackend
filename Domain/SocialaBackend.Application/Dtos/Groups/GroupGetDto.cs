using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class GroupGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int UsersCount { get; set; }
        public string ConnectionId { get; set; } = null!;
        public ICollection<GroupMembersGetDto> Members { get; set; } = new List<GroupMembersGetDto>();
        public ICollection<GroupMessageGetDto> Messages { get; set; } = new List<GroupMessageGetDto>();
    }
}
