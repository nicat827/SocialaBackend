using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Chat:BaseEntity
    {
        //relational

        public string FirstUserId { get; set; } = null!;
        public AppUser FirstUser { get; set; } = null!;
        public string SecondUserId { get; set; } = null!;
        public AppUser SecondUser { get; set;} = null!;
        
        public string? LastMessage { get; set; }
        public DateTime? LastMessageSendedAt { get; set; }

    }
}
