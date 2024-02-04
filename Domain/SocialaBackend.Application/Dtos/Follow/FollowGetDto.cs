using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record FollowGetDto(int Id,string Name, string Surname, string UserName, string? ImageUrl, bool IsConfirmed);
    
}
