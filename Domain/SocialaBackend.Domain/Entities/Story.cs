using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Story:BaseEntity
    {
        //ralational

        public string OwnerId { get; set; } = null!;
        public AppUser Owner { get; set; } = null!;

        public DateTime? LastItemAddedAt { get; set; }
        public ICollection<StoryItem> StoryItems { get; set; } = new List<StoryItem>();

        public int LastStoryItemId { get; set; }
    }
}
