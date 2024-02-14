using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class NotificationsCheckDto
    {
        public ICollection<int> Notifications { get; set; } = null!;
    }
}
