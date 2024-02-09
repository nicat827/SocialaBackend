using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class NotificationsGetDto
    {
        public string? Title { get; set; }

        public string Text { get; set; } = null!;

        public string? SourceUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
