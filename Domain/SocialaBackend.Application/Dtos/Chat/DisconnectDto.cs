﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class DisconnectDto
    {
        public int ChatId { get; set; }

        public string UserName { get; set; } = null!;
    }
}
