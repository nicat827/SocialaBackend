using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions.Chat
{
    public class SendMessageException : BaseException
    {
        public SendMessageException(string mess) : base(mess, 400)
        {
            
        }
    }
}
