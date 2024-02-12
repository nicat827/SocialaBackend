﻿using SocialaBackend.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities
{
    public class Message:BaseEntity
    {
        public string Text { get; set; } = null!;

        public string SendedBy { get; set; } = null!;

        public bool IsChecked { get; set; }
        //relational
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
    }
}
