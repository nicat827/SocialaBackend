using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly string _currentUsername;
        public ChatService(IMessageRepository messageRepository, IHttpContextAccessor http, IChatRepository chatRepository, UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _userManager = userManager;
            _currentUsername = http.HttpContext.User.Identity.Name;
        }
        public async Task<ChatGetDto> GetChatByIdAsync(int id)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(id, expressionIncludes: c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(20), includes: new[] { "FirstUser", "SecondUser" });
            if (chat is null) throw new NotFoundException($"Chat with id {id} doesnt exits!");
            if (chat.FirstUser.UserName != _currentUsername || chat.SecondUser.UserName != _currentUsername)
                throw new DontHavePermissionException("You cant get this chat!");
            var firstUser = chat.FirstUser;
            //var messages = chat.Messages.OrderByDescending(m => m.CreatedAt);
            ICollection<MessageGetDto> messagesDto = new List<MessageGetDto>();
            foreach (Message message in  chat.Messages) messagesDto.Add(new MessageGetDto { Text = message.Text, IsChecked = message.IsChecked}); 
            return new ChatGetDto
            {
                ChatPartnerImageUrl = firstUser.UserName == _currentUsername ? chat.SecondUser.ImageUrl : chat.FirstUser.ImageUrl,
                ChatPartnerUserName = firstUser.UserName == _currentUsername ? chat.SecondUser.UserName : chat.FirstUser.UserName,
                Messages = messagesDto
                
            };
        }

        public async Task<MessageGetDto> SendMessageAsync(MessagePostDto dto)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(dto.ChatId, includes:new[] { "FirstUser", "SecondUser" });
            if (chat is null) throw new NotFoundException($"Chat with id {dto.ChatId} doesnt exists!");
            if (chat.FirstUser.UserName != _currentUsername || chat.SecondUser.UserName != _currentUsername)
                throw new DontHavePermissionException("You cant write message to this chat!");
            Message message = new Message
            {
                Text = dto.Text,
                SendedBy = _currentUsername,
                ChatId = chat.Id
            };
            chat.LastMessage = dto.Text;
            chat.LastMessageSendedAt = DateTime.Now;
            chat.LastMessageSendedBy = _currentUsername;
            await _messageRepository.CreateAsync(message);
            await _messageRepository.SaveChangesAsync();
            return new MessageGetDto { CreatedAt = message.CreatedAt, Id = message.Id, Text = message.Text };
        }

        public async Task SendMessageFromProfileAsync(MessagePostDtoFromProfile dto)
        {
            AppUser? receiver = await _userManager.Users
                .Where(u => dto.ReceiverUsername == u.UserName)
                .Include(u => u.Chats)
                .Include(u => u.Followers)
                .FirstOrDefaultAsync();

            if (receiver is null) throw new NotFoundException($"Receiver with username {dto.ReceiverUsername} doesnt exits!");
            if (receiver.IsPrivate && !receiver.Followers.Any(f => f.UserName == _currentUsername && f.IsConfirmed == true))
                throw new DontHavePermissionException("Follow first, for send messages!");
            Chat? chat = await _chatRepository.Get(c => c.FirstUser.UserName == _currentUsername && c.SecondUser.UserName == receiver.UserName
                                                    || c.FirstUser.UserName == receiver.UserName && c.SecondUser.UserName == _currentUsername,
                                                    includes: new[] {"FirstUser", "SecondUser"});
            if (chat is null)
            {
                chat = new Chat
                {
                    FirstUserId = _currentUsername,
                    SecondUserId = receiver.Id,

                };
            }
            Message newMessage = new Message
            {
                Chat = chat,
                SendedBy = _currentUsername,
                Text = dto.Text,
            };
            chat.LastMessage = newMessage.Text;
            chat.LastMessageSendedAt = DateTime.Now;
            chat.LastMessageSendedBy = _currentUsername;
            await _chatRepository.CreateAsync(chat);
            await _messageRepository.CreateAsync(newMessage);
            await _chatRepository.SaveChangesAsync();


        }

        public async Task<ICollection<ChatItemGetDto>> GetChatItemsAsync()
        {
            ICollection<Chat> userChats = await _chatRepository.GetCollection(
                c => c.FirstUser.UserName == _currentUsername
                || c.SecondUser.UserName == _currentUsername,
                includes: new[] { "FirstUser", "SecondUser" });
            var orderedChats = userChats.OrderByDescending(c => c.LastMessageSendedAt);
            ICollection<ChatItemGetDto> dto = new List<ChatItemGetDto>();
            foreach (Chat chat in orderedChats)
            {
                if (chat.FirstUser.UserName == _currentUsername)
                {
                    dto.Add(new ChatItemGetDto
                    {
                        ChatId = chat.Id,
                        ChatPartnerUserName = chat.SecondUser.UserName,
                        ChatPartnerImageUrl = chat.SecondUser.ImageUrl,
                        LastMessage = chat.LastMessage,
                        LastMessageSendedAt = chat.LastMessageSendedAt,
                        LastMessageSendedBy = chat.LastMessageSendedBy
                    });
                }
                else
                {
                    dto.Add(new ChatItemGetDto
                    {
                        ChatId = chat.Id,
                        ChatPartnerUserName = chat.FirstUser.UserName,
                        ChatPartnerImageUrl = chat.FirstUser.ImageUrl,
                        LastMessage = chat.LastMessage,
                        LastMessageSendedAt = chat.LastMessageSendedAt,
                        LastMessageSendedBy = chat.LastMessageSendedBy
                    });
                }
            }

            return dto;
        }
    }
}
