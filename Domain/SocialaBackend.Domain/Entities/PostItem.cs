using SocialaBackend.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class PostItem:BaseEntity
    {
        public string SourceUrl { get; set; } = null!;

        public int PostId { get; set; }

        public Post Post { get; set; } = null!;
    }
}
