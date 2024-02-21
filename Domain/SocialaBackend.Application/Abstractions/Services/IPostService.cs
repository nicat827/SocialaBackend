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
        Task<PostGetDto> CreatePostAsync(PostPostDto dto);

        Task<ICollection<PostGetDto>> GetPostsAsync(string username);

        Task LikePostAsync(int id);
        Task<IEnumerable<ReplyGetDto>> GetRepliesAsync(int id, int? skip);

        Task<IEnumerable<PostGetDto>> GetArchivedPostsAsync(int skip);

        Task<ICollection<PostLikeGetDto>> GetLikesAsync(int id, int? skip);
        Task LikeCommentAsync(int id);
        Task LikeReplyAsync(int id);
        Task<IEnumerable<CommentGetDto>> GetCommentsAsync(int id, int? skip);
        Task RecoverPostAsync(int id);

        Task<IEnumerable<PostGetDto>> GetFeedPostsAsync(int skip);
        Task<CommentGetDto> CommentAsync(CommentPostDto dto);
        Task DeletePostAsync(int id);
        Task<ReplyGetDto> ReplyCommentAsync(ReplyPostDto dto);
    }
}
