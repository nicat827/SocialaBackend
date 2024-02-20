using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class NotificationsGetDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool IsChecked { get; set; }
        public string Text { get; set; } = null!;
        public string? UserName { get; set; } = null!;

        public string? SourceUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Type { get; set; } = null!;

        public int? SrcId { get; set; }

    }
}
