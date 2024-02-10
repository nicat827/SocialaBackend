using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class StoryGetDto
    {
        public int Id { get; set; } 

        public string? OwnerImageUrl { get; set; }

        public string OwnerUserName { get; set; } = null!;

        public DateTime? LastStoryPostedAt { get; set; }

        
    }
}
