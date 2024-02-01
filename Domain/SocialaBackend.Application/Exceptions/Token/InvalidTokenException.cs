using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions.Token
{
    public class InvalidTokenException : BaseException
    {
        public InvalidTokenException(string mess, int code=400) : base(mess, code)
        {
        }
    }
}
