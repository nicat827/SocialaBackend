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
        public string AppUserId = null!;
        public AppUser AppUser { get; set; } = null!;

        //ralational

        public ICollection<StoryItem> StoryItems { get; set; } = new List<StoryItem>();
    }
}
