using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class GroupCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string OwnerUserName { get; set; } = null!;
        public IFormFile? Photo { get; set; }
        public ICollection<GroupMembersPostDto> Members { get; set; } = new List<GroupMembersPostDto>();
    }
}
