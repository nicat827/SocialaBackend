﻿using Microsoft.AspNetCore.Http;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat;
using SocialaBackend.Application.Dtos.Chat.Message;
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
        Task<ICollection<ChatItemSearchGetDto>> SearchChatUsersAsync(string searchParam, string currentUsername);
        Task<(MessageGetDto, ChatGetDto)> SendMessageFromProfileAsync(MessagePostDtoFromProfile dto);
        Task<ICollection<ChatItemGetDto>> GetChatItemsAsync(string userName);

        Task<ChatDeleteGetDto> DeleteMessageAsync(int id, string userName);
        Task<ChatGetDto> GetChatByIdAsync(int id, string userName);
        Task SendAudioAsync(AudioMessagePostDto dto);
        Task<int> GetUnreadedMessagesCountAsync(string userName);
        Task<IEnumerable<MessageGetDto>> GetMessagesAsync(int chatId, string userName, int skip);
        Task<int> GetNewMessagesCountAsync(string userName);

        Task SendMediaAsync(string userName, int chatId, ICollection<MediaMessagePostDto> medias);
        Task<int> GetChatsCountAsync(string userName);
    }
}
