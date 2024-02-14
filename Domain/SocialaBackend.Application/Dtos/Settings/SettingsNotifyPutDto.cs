using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class SettingsNotifyPutDto
    {
        public bool PhotoLikeNotify { get; set; }
        public bool PostLikeNotify { get; set; } 
        public bool PostCommentNotify { get; set; } 
        public bool FollowerNotify { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
