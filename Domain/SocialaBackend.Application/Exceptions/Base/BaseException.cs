using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions.Base
{
    public abstract class BaseException:Exception
    {
        public BaseException(string mess, int code): base(mess)
        {
            Code = code;
        }

        public int Code { get; set; }
    }
}
