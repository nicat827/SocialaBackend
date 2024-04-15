using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MediaMessagePostDto
    {
        public IFormFile File { get; set; } = null!;
        public string? Text { get; set; }
    }
}
