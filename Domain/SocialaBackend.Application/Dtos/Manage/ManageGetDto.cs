using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class ManageGetDto
    {
        public StatOfRegisteredUsersDto RegisteredUsersCountByGender { get; set; } = null!;
        
        public int AllUsersCount { get; set; }
        public int AdminsCount { get; set; }
        public int ModeratorsCount { get; set; }
        public int VerifiedUsersCount { get; set; }

    }
}
