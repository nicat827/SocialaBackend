using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities.User
{
    public class AppUser:IdentityUser
    {
        
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public string? RefreshToken { get; set; } = null!;

        public DateTime? RefreshTokenExpiresAt { get; set; }

        //relational
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
