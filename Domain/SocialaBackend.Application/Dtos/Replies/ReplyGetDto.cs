using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record ReplyGetDto(int Id, string Author, string? AuthorImageUrl, string Text, int LikesCount);
}
