using SocialaBackend.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class StoryItem:BaseEntity
    {
        public string? SourceUrl { get; set; }
        public int WatchCount { get; set; }
        public string? Text { get; set; }

        //relational
        public int StoryId { get; set; }

        public Story Story { get; set; } = null!;

        public ICollection<StoryItemWatcher> Watchers { get; set; } = new List<StoryItemWatcher>();
    }
}
