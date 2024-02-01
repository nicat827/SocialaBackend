﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities.User
{
    public class AppUser:IdentityUser
    {
        
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;

        public IFormFile? Photo { get; set; }
    }
}
