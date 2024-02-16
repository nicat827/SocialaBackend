using Microsoft.AspNetCore.Components;
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
        public ChatService(IMessageRepository messageRepository, IChatRepository chatRepository, UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _userManager = userManager;


        }
        public async Task<ChatGetDto> GetChatByIdAsync(int id, string userName)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(id, expressionIncludes: c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(20), includes: new[] { "FirstUser", "SecondUser" });
            if (chat is null) throw new NotFoundException($"Chat with id {id} doesnt exits!");
            if (chat.FirstUser.UserName != userName && chat.SecondUser.UserName != userName)
                throw new DontHavePermissionException("You cant get this chat!");
            var firstUser = chat.FirstUser;
            //var messages = chat.Messages.OrderByDescending(m => m.CreatedAt);
            ICollection<MessageGetDto> messagesDto = new List<MessageGetDto>();
            foreach (Message message in  chat.Messages) messagesDto.Add(new MessageGetDto {Id=message.Id, CreatedAt = message.CreatedAt, Sender = message.SendedBy, Text = message.Text, IsChecked = message.IsChecked}); 
            return new ChatGetDto
            {
                Id = chat.Id,
                ChatPartnerName = firstUser.UserName == userName ? chat.SecondUser.Name : chat.FirstUser.Name,
                ChatPartnerSurname = firstUser.UserName == userName ? chat.SecondUser.Surname : chat.FirstUser.Surname,
                ChatPartnerImageUrl = firstUser.UserName == userName ? chat.SecondUser.ImageUrl : chat.FirstUser.ImageUrl,
                ChatPartnerUserName = firstUser.UserName == userName ? chat.SecondUser.UserName : chat.FirstUser.UserName,
                Messages = messagesDto,
                ConnectionId = chat.ConnectionId,
            };
        }

        public async Task<ICollection<MessageGetDto>> GetMessagesAsync(int chatId,string userName, int skip)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(chatId, expressionIncludes: c => c.Messages.OrderByDescending(m => m.CreatedAt).Skip(skip).Take(20), includes: new[] { "FirstUser", "SecondUser" });
            if (chat is null || chat.FirstUser.UserName != userName && chat.SecondUser.UserName != userName)
                throw new DontHavePermissionException("You cant get this chat!");
            ICollection<MessageGetDto> messagesDto = new List<MessageGetDto>();

            foreach (Message message in chat.Messages) messagesDto.Add(new MessageGetDto { 
                Id = message.Id,
                CreatedAt = message.CreatedAt,
                Sender = message.SendedBy,
                Text = message.Text,
                IsChecked = message.IsChecked });
            return messagesDto;
        }


        public async Task<int> GetNewMessagesCountAsync(string userName)
        {
            ICollection<Chat> chats = await _chatRepository.GetCollection(c => c.FirstUser.UserName == userName 
            || c.SecondUser.UserName == userName,includes:new[] { "FirstUser", "SecondUser", "Messages" });
            int count = 0;
            foreach (Chat chat in chats)
            {
                count += chat.Messages.Where(m => m.IsChecked == false && m.SendedBy != userName).Count();
            }
            return count;
                
        }

        public async Task<ICollection<ChatItemSearchGetDto>> SearchChatUsersAsync(string searchParam, string currentUsername)
        {
            AppUser? currentUser = await _userManager.Users
                .Where(u => u.UserName == currentUsername)
                .Include(u => u.Followers.Where(f => f.IsConfirmed == true))
                .Include(u => u.Follows.Where(f => f.IsConfirmed == true))
                .FirstOrDefaultAsync();
            if (currentUser is null) throw new AppUserNotFoundException($"User with username {currentUser} doesnt exists!");

            ICollection<FollowerItem> filteredFollowers = currentUser.Followers.Where(f => 
            f.UserName.Contains(searchParam) 
            || f.Name.ToLower().Contains(searchParam.ToLower())
            || f.Surname.ToLower().Contains(searchParam.ToLower())
            ).ToList();

            ICollection<FollowItem> filteredFollows = currentUser.Follows.Where(f =>
            f.UserName.Contains(searchParam)
            || f.Name.ToLower().Contains(searchParam.ToLower())
            || f.Surname.ToLower().Contains(searchParam.ToLower())
            ).ToList();
            ICollection<ChatItemSearchGetDto> dto = new List<ChatItemSearchGetDto>();

            foreach (FollowerItem follower in filteredFollowers)
            {
                Chat? chat = await _chatRepository.Get(
                    c => c.FirstUser.UserName == currentUsername && c.SecondUser.UserName == follower.UserName ||
                    c.FirstUser.UserName == follower.UserName && c.SecondUser.UserName == currentUsername);
                dto.Add(new ChatItemSearchGetDto
                {
                    ChatId = chat is null ? null : chat.Id,
                    ImageUrl = follower.ImageUrl,
                    Name = follower.Name,
                    Surname = follower.Surname,
                    UserName = follower.UserName
                });
            }
            foreach (FollowItem follow in filteredFollows.Where(f => !filteredFollowers.Any(fi => fi.UserName == f.UserName)))
            {
                Chat? chat = await _chatRepository.Get(
                    c => c.FirstUser.UserName == currentUsername && c.SecondUser.UserName == follow.UserName ||
                    c.FirstUser.UserName == follow.UserName && c.SecondUser.UserName == currentUsername);
                dto.Add(new ChatItemSearchGetDto
                {
                    ChatId = chat is null ? null : chat.Id,
                    ImageUrl = follow.ImageUrl,
                    Name = follow.Name,
                    Surname = follow.Surname,
                    UserName = follow.UserName
                });
            }
            return dto;


        }

        public async Task<MessageGetDto> SendMessageAsync(MessagePostDto dto)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(dto.ChatId,true, includes:new[] { "FirstUser", "SecondUser" });
            if (chat is null) throw new NotFoundException($"Chat with id {dto.ChatId} doesnt exists!");
            if (chat.FirstUser.UserName != dto.Sender && chat.SecondUser.UserName != dto.Sender)
                throw new DontHavePermissionException("You cant write message to this chat!");
            Message message = new Message
            {
                Text = dto.Text,
                SendedBy = dto.Sender,
                ChatId = chat.Id
            };
            chat.LastMessage = dto.Text;
            chat.LastMessageSendedAt = DateTime.Now;
            chat.LastMessageSendedBy = dto.Sender;
            chat.LastMessageIsChecked = false;
            await _messageRepository.CreateAsync(message);
            await _messageRepository.SaveChangesAsync();
            return new MessageGetDto { CreatedAt = message.CreatedAt, Id = message.Id, Text = message.Text, Sender = message.SendedBy };
        }


        public async Task<(MessageGetDto, ChatGetDto)> SendMessageFromProfileAsync(MessagePostDtoFromProfile dto)
        {
            AppUser? receiver = await _userManager.Users
                .Where(u => dto.ReceiverUsername == u.UserName)
                .Include(u => u.Chats)
                .Include(u => u.Followers)
                .FirstOrDefaultAsync();
            AppUser? sender = await _userManager.FindByNameAsync(dto.Sender);
            if (sender == null) throw new AppUserNotFoundException("Sender wasnt found!");
            if (receiver is null) throw new NotFoundException($"Receiver with username {dto.ReceiverUsername} doesnt exits!");
            if (receiver.IsPrivate && !receiver.Followers.Any(f => f.UserName == dto.Sender && f.IsConfirmed == true))
                throw new DontHavePermissionException("Follow first, for send messages!");
            Chat? chat = await _chatRepository.Get(c => c.FirstUser.UserName == dto.Sender && c.SecondUser.UserName == receiver.UserName
                                                    || c.FirstUser.UserName == receiver.UserName && c.SecondUser.UserName == dto.Sender,
                                                    isTracking:true,
                                                    includes: new[] {"FirstUser", "SecondUser"});
            bool isNew = false;
            if (chat is null)
            {
                isNew = true;
                chat = new Chat
                {
                    FirstUserId = sender.Id,
                    SecondUserId = receiver.Id,
                    ConnectionId = Guid.NewGuid().ToString()

                };
            }
            Message newMessage = new Message
            {
                Chat = chat,
                SendedBy = dto.Sender,
                Text = dto.Text,
            };
            chat.LastMessage = newMessage.Text;
            chat.LastMessageSendedAt = DateTime.Now;
            chat.LastMessageSendedBy = dto.Sender;
            chat.LastMessageIsChecked = false;
            if (isNew) await _chatRepository.CreateAsync(chat);

            await _messageRepository.CreateAsync(newMessage);
            await _chatRepository.SaveChangesAsync();
            MessageGetDto messageDto = new MessageGetDto { CreatedAt = newMessage.CreatedAt, Id = newMessage.Id, Text = newMessage.Text, Sender = newMessage.SendedBy };
            var firstUser = chat.FirstUser;
            ChatGetDto chatDto = new ChatGetDto
            {
                ChatPartnerImageUrl = firstUser.UserName == dto.Sender ? chat.SecondUser.ImageUrl : chat.FirstUser.ImageUrl,
                ChatPartnerUserName = firstUser.UserName == dto.Sender ? chat.SecondUser.UserName : chat.FirstUser.UserName,
                ChatPartnerName = firstUser.UserName == dto.Sender ? chat.SecondUser.Name : chat.FirstUser.Name,
                ChatPartnerSurname = firstUser.UserName == dto.Sender ? chat.SecondUser.Surname : chat.FirstUser.Surname,
                Messages = new List<MessageGetDto>(),
                ConnectionId = chat.ConnectionId,
                Id = chat.Id,
            };
            return (messageDto, chatDto);



        }

        public async Task<ICollection<ChatItemGetDto>> GetChatItemsAsync(string userName)
        {
            ICollection<Chat> userChats = await _chatRepository.OrderAndGet(
                order: c => c.LastMessageSendedAt,
                isDescending: true,
                expression: c => c.FirstUser.UserName == userName
                || c.SecondUser.UserName == userName,
                expressionIncludes: c => c.Messages.Where(m => !m.IsChecked && m.SendedBy != userName),
                includes: new[] { "FirstUser", "SecondUser" }).ToListAsync();

            ICollection<ChatItemGetDto> dto = new List<ChatItemGetDto>();
            foreach (Chat chat in userChats)
            {
                if (chat.FirstUser.UserName == userName)
                {
                    dto.Add(new ChatItemGetDto
                    {
                        ChatId = chat.Id,
                        ChatPartnerUserName = chat.SecondUser.UserName,
                        ChatPartnerName = chat.SecondUser.Name,
                        ChatPartnerSurname =chat.SecondUser.Surname,
                        ChatPartnerImageUrl = chat.SecondUser.ImageUrl,
                        LastMessage = chat.LastMessage,
                        UnreadedMessagesCount = chat.Messages.Count,
                        LastMessageIsChecked = chat.LastMessageIsChecked,
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
                        ChatPartnerName = chat.FirstUser.Name,
                        ChatPartnerSurname = chat.FirstUser.Surname,
                        ChatPartnerImageUrl = chat.FirstUser.ImageUrl,
                        LastMessage = chat.LastMessage,
                        UnreadedMessagesCount = chat.Messages.Count,
                        LastMessageIsChecked = chat.LastMessageIsChecked,
                        LastMessageSendedAt = chat.LastMessageSendedAt,
                        LastMessageSendedBy = chat.LastMessageSendedBy
                    });
                }
            }

            return dto;
        }
    }
}
