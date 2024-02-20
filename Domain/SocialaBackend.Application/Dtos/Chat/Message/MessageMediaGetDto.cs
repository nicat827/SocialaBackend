using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MessageMediaGetDto
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; } = null!;
        public FileType MediaType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
