﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record AppUserGetDto(string Name, string Surname, string UserName, string ImageUrl);
    
}
