using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class VerifyRequestGetDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Fullname { get; set; } = null!;
        public int FollowersCount { get; set; }
    }

}
