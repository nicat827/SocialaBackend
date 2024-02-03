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

        Task CommentAsync(int id, string text, string username);
    }
}
