using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class StoryItemWatcherDto
    {
        public int Id { get; set; }
        public string? WatcherImageUrl { get; set; }
        public string WatcherUserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
