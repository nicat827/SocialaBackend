﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class ManageGetDto
    {
        public StatOfRegisteredUsersDto RegisteredUsersCountByGender { get; set; } = null!;


    }
}