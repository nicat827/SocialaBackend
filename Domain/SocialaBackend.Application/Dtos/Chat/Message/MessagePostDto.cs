﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MessagePostDto
    {
        public int ChatId { get; set; }
        public string Text { get; set; } = null!;
        //public ICollection<MediaPostDto>? Media { get; set; }

        public string Sender { get; set; } = null!;
    }
}
