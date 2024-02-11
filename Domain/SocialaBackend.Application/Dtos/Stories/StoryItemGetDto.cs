using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class StoryItemGetDto
    {
        public int Id { get; set; }
        public string SourceUrl { get; set; } = null!;

        public FileType Type { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
