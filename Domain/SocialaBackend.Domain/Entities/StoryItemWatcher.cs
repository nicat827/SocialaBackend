using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class StoryItemWatcher:BaseEntity
    {
        //raltional 
        public int StoryItemId { get; set; }

        public StoryItem StoryItem { get; set; } = null!;

        public string? WatcherId { get; set; }
        public AppUser? Watcher { get; set; }
    }
}
