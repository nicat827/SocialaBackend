﻿using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IChatService
    {
        Task<MessageGetDto> SendMessageAsync(MessagePostDto dto);

        Task SendMessageFromProfileAsync(MessagePostDtoFromProfile dto);
        Task<ICollection<ChatItemGetDto>> GetChatItemsAsync();
        Task<ChatGetDto> GetChatByIdAsync(int id);
    }
}
