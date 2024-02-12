using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MessagePostDtoFromProfile
    {
        public string ReceiverUsername { get; set; } = null!;

        public string Text { get; set; } = null!;

        public string Sender { get; set; } = null!;

    }
}
