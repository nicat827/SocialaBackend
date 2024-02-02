using Microsoft.AspNetCore.Identity;
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

        public PostService(UserManager<AppUser> userManager, IFIleService fileService, IPostRepository repository)
        {
            _userManager = userManager;
            _fileService = fileService;
            _repository = repository;
        }
        public async Task CreatePostAsync(string username , PostPostDto dto)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException("User with this username wasnt found!");
            _fileService.CheckFileSize(dto.File, 5);

            string type = dto.File.ContentType.Substring(0, dto.File.ContentType.IndexOf("/"));
            Console.WriteLine(type);
            Post newPost = new Post
            {
                Description = dto.Description,
                SourceUrl = await _fileService.CreateFileAsync(dto.File, "uploads", "posts", $"{type}s"),
                ApppUserId = user.Id
            };

            await _repository.CreateAsync(newPost);
            await _repository.SaveChangesAsync();

        }
    }
}
