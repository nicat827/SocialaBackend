using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class FollowerItem:BaseEntity
    {
        public string UserName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public bool IsConfirmed { get; set; }

        //relational
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;

    }
}
