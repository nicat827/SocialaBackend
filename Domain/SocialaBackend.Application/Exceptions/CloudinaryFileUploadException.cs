using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions
{
    public class CloudinaryFileUploadException : BaseException
    {
        public CloudinaryFileUploadException(string mess, int code=500) : base(mess, code)
        {
        }
    }
}
