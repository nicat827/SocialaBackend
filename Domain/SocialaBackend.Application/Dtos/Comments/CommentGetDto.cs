﻿using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public record CommentGetDto(int Id, string Author, string? AuthorImageUrl, string Text, int LikesCount, int RepliesCount, DateTime CreatedAt, int? ParentCommentId);
    
}
