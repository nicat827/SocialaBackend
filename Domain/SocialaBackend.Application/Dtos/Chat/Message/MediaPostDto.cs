using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class MediaPostDto
    {
        public byte[] MediaInBytes { get; set; } = null!;

        public string FileName { get; set; } = null!;
    }
}
