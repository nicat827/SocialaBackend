using SocialaBackend.Domain.Entities.Base;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class MessageMedia:BaseEntity
    {
        public string MediaUrl { get; set; } = null!;
        public FileType MediaType { get; set; }
        //realtional
        public int MessageId { get; set; }
        public Message Message { get; set; } = null!;
    }
}
