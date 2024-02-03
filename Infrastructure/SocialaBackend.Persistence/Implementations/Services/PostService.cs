using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class PostService : IPostService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFIleService _fileService;
        private readonly IPostRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;

        public PostService(UserManager<AppUser> userManager,
            IFIleService fileService,
            IPostRepository repository,
            IMapper mapper,
            ICommentRepository commentRepository,
            IPostRepository postRepository)
        {
            _userManager = userManager;
            _fileService = fileService;
            _repository = repository;
            _mapper = mapper;
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        public async Task CommentAsync(int id, string text, string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Post post = await _postRepository.GetByIdAsync(id,isTracking:true, includes:"Comments");
            if (post is null) throw new NotFoundException("Post didnt found!");

            post.Comments.Add(new Comment
            {
                Text = text,
                AuthorImageUrl = user.ImageUrl,
                Author = user.UserName
            });
            await _postRepository.SaveChangesAsync();

        }

        public async Task CreatePostAsync(string username , PostPostDto dto)
        {
            AppUser user = await _getUser(username);
            Post newPost = new Post
            {
                Description = dto.Description,
                Items = new List<PostItem>(),
                AppUserId = user.Id
            };
            foreach (var file in dto.Files)
            {
                _fileService.CheckFileSize(file, 15);
                _fileService.ValidateFilesForPost(file);
                string type = file.ContentType.Substring(0, file.ContentType.IndexOf("/"));
                newPost.Items.Add(new PostItem { SourceUrl = await _fileService.CreateFileAsync(file, "uploads", "posts", $"{type}s") });
            }

            await _repository.CreateAsync(newPost);
            await _repository.SaveChangesAsync();

        }

       

        public async Task<ICollection<PostGetDto>> GetPostsAsync(string username)
        {
            AppUser? user = await _userManager.Users
                .Where(u => u.UserName == username)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.Likes)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.Replies)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Likes)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Items)
                .FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            ICollection<PostGetDto> dto = _mapper.Map<ICollection<PostGetDto>>(user.Posts);
            return dto;

        }

        public async Task LikePostAsync(int id, string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Post post = await _postRepository.GetByIdAsync(id, isTracking: true, false, "Likes");
            if (post is null) throw new NotFoundException("Post didnt found!");

            PostLikeItem? likedItem = post.Likes.FirstOrDefault(li => li.AppUserId == user.Id);
            if (likedItem is null)
            {
                post.Likes.Add(new PostLikeItem {AppUserId = user.Id, Username =user.UserName, ImageUrl = user.ImageUrl, Name = user.Name, Surname = user.Surname });

            }
            else post.Likes.Remove(likedItem);
            await _repository.SaveChangesAsync();
        }

        private async Task<AppUser> _getUser(string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            return user;
        }
    }
}
