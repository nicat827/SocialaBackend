using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class AppUserSearchDto
    {
        public string UserName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
