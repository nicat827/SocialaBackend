using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions
{
    public class AppUserCreateException : BaseException
    {
        public AppUserCreateException(string mess, int code= 400) : base(mess, code)
        {
        }
    }
}
