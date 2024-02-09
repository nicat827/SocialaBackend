using Microsoft.AspNetCore.Http;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class SettingsDescriptionPutDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public IFormFile? Photo { get; set; }
        public bool IsPrivate { get; set; } 
        public string Email { get; set; } = null!;
        public string? Bio {  get; set; }
        public Gender Gender { get; set; }
    }
}
