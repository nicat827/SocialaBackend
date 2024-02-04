using SocialaBackend.Application.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Exceptions.AppUser
{
    public class AppUserLockoutException : BaseException
    {
        public AppUserLockoutException(string mess, byte totalMinutes, byte totalSeconds, int code=423) : base(mess, code)
        {
            TotalSecunds = totalSeconds;
            TotalMinutes = totalMinutes;
        }
        public byte TotalMinutes { get; set; }

        public byte TotalSecunds { get; set; }
    }
}
