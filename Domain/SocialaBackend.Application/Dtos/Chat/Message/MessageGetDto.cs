using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MessageGetDto
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Text { get; set; } = null!;
        public bool IsChecked { get; set; } 
    }
}
