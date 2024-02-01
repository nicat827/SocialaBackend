using Microsoft.AspNetCore.Identity;
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

        public PostService(UserManager<AppUser> userManager, IFIleService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
        }
        public async Task CreatePostAsync(string username , PostPostDto dto)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException("User with this name wasnt found!");
            _fileService.CheckFileSize(dto.File, 5);
            


        }
    }
}
