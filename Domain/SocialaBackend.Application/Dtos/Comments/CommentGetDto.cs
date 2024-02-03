using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record CommentGetDto(string Author, string? AuthorImageUrl, string Text);
    
}
