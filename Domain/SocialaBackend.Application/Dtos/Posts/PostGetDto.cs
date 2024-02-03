using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record PostGetDto(int Id, string? Description, DateTime CreatedAt, ICollection<PostItemGetDto> Items, ICollection<CommentGetDto> Comments, ICollection<PostLikeItem> Likes);
}
