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

        public PostService(UserManager<AppUser> userManager, IFIleService fileService, IPostRepository repository, IMapper mapper)
        {
            _userManager = userManager;
            _fileService = fileService;
            _repository = repository;
            _mapper = mapper;
        }
        public async Task CreatePostAsync(string username , PostPostDto dto)
        {
            AppUser user = await _getUser(username);
            Post newPost = new Post
            {
                Description = dto.Description,
                PostItems = new List<PostItem>(),
                AppUserId = user.Id
            };
            foreach (var file in dto.Files)
            {
                _fileService.CheckFileSize(file, 15);
                _fileService.ValidateFilesForPost(file);
                string type = file.ContentType.Substring(0, file.ContentType.IndexOf("/"));
                newPost.PostItems.Add(new PostItem { SourceUrl = await _fileService.CreateFileAsync(file, "uploads", "posts", $"{type}s") });
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
                    .ThenInclude(p => p.PostItems)
                .FirstOrDefaultAsync();
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            ICollection<PostGetDto> dto = _mapper.Map<ICollection<PostGetDto>>(user.Posts);
            return dto;

        }

        private async Task<AppUser> _getUser(string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            return user;
        }
    }
}
