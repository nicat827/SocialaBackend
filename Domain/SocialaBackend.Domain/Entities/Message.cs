using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Message:BaseEntity
    {
        public string? Text { get; set; }
        public string Sender { get; set; } = null!;
        public string? AudioUrl { get; set; }
        public string? SourceUrl { get; set; }
        public bool IsChecked { get; set; }
        public FileType Type { get; set; }
        public int? AudioMinutes { get; set; }
        public int? AudioSeconds { get; set; }
        //relational
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
    }
}
