using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record PostLikeGetDto(int Id, string Username, string Name, string Surname, string? ImageUrl);
    
}
