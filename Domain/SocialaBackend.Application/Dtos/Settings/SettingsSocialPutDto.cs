using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class SettingsSocialPutDto
    {
        public string? FacebookLink { get; set; }
        public string? GithubLink { get; set; }
        public string? InstagramLink { get; set; }
    }
}
