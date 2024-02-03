using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string mess, int code=404) : base(mess, code)
        {
        }
    }
}
