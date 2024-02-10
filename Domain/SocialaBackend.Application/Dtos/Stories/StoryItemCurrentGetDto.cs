using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class StoryItemCurrentGetDto
    {
        public int Id { get; set; }
        public string SourceUrl { get; set; } = null!;
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int WatchCount { get; set; }
        public IEnumerable<StoryItemWatcher> Watchers { get; set; } = new List<StoryItemWatcher>();
        public string UserName { get; set; } = null!;
        public string? ImageUrl { get; set; }

    }
}
