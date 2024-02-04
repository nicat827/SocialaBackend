using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IPostService
    {
        Task CreatePostAsync(string username, PostPostDto dto);

        Task<ICollection<PostGetDto>> GetPostsAsync(string username);

        Task LikePostAsync(int id, string username);
        Task<IEnumerable<ReplyGetDto>> GetRepliesAsync(int id, int? skip);

        Task<IEnumerable<PostLikeGetDto>> GetLikesAsync(int id, int? skip);
        Task LikeCommentAsync(int id, string username);
        Task LikeReplyAsync(int id, string username);
        Task<IEnumerable<CommentGetDto>> GetCommentsAsync(int id, int? skip);

        Task CommentAsync(int id, string text, string username);
        Task ReplyCommentAsync(int id, string text, string username);
    }
}
