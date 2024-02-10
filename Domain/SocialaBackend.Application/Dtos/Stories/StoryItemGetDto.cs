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
        public DateTime CreatedAt { get; set; }

        public string UserName { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}
