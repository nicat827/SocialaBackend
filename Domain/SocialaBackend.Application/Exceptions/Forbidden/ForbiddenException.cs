using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions.Forbidden
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string mess, int code= 403) : base(mess, code)
        {
        }
    }
}
