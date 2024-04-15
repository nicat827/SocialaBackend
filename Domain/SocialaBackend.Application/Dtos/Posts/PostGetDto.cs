using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record PostGetDto(int Id, string AppUserUserName,string AppUserName,string AppUserSurname,string? AppUserImageUrl, string? Description, DateTime CreatedAt, IEnumerable<PostItemGetDto> Items, IEnumerable<CommentGetDto> Comments, int CommentsCount, int RepliesCount, int LikesCount);
}
