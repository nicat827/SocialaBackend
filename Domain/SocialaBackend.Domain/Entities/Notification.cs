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
    public class Notification:BaseEntity
    {
        public string Text { get; set; } = null!;
        public string? Title { get; set; }
        public string? SourceUrl { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public NotificationType Type { get; set; }
        public bool IsChecked { get; set; }
        public string UserName { get; set; } = null!;
        public int? SrcId { get; set; }
    }
}
