using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MessageGetDto
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Text { get; set; }
        public string? AudioUrl { get; set; }
        public int? AudioMinutes { get; set; }
        public int? AudioSeconds { get; set; }
        public string? SourceUrl { get; set; }
        public FileType Type { get; set; }
        public bool IsChecked { get; set; }
        public string Sender { get; set; } = null!;
    }
}
