using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class SettingsDescriptionGetDto
    {
        public string? ImageUrl { get; set; }
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Bio { get; set; }

        public Gender Gender { get; set; }
    }
}
