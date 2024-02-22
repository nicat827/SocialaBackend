using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class ChatDeleteGetDto
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; } = null!;
        public string FirstUserUserName { get; set; } = null!;
        public string SecondUserUserName { get; set; } = null!;
        public IEnumerable<MessageGetDto> Messages { get; set; } = new List<MessageGetDto>();

    }
}
