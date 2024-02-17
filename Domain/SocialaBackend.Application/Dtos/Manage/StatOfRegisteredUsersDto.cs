using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class StatOfRegisteredUsersDto
    {
        public int MaleCount { get; set; }

        public int FemaleCount { get; set; }

        public int OtherCount { get; set; }
    }
}
