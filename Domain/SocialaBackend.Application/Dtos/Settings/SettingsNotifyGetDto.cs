using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos.Settings
{
    public class SettingsNotifyGetDto
    {
        public bool PhotoLikeNotify { get; set; }
        public bool PostLikeNotify { get; set; }
        public bool FollowerNotify { get; set; }
    }
}
