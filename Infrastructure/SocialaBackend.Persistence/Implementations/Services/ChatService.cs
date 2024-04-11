using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Base;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.Implementations.Hubs;
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
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IFileService _fileService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly UserManager<AppUser> _userManager;
        public ChatService(IMapper mapper, INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext, IFileService fileService, ICloudinaryService cloudinaryService, IMessageRepository messageRepository, IChatRepository chatRepository, UserManager<AppUser> userManager)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _fileService = fileService;
            _cloudinaryService = cloudinaryService;
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _userManager = userManager;


        }
        public async Task<ChatGetDto> GetChatByIdAsync(int id, string userName)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(id, expressionIncludes: c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(20), includes: new[] { "FirstUser", "SecondUser", "Messages.Media" });
            if (chat is null) throw new NotFoundException($"Chat with id {id} doesnt exits!");
            if (chat.FirstUser.UserName != userName && chat.SecondUser.UserName != userName)
                throw new DontHavePermissionException("You cant get this chat!");
            var firstUser = chat.FirstUser;
            //var messages = chat.Messages.OrderByDescending(m => m.CreatedAt);
            
            return new ChatGetDto
            {
                Id = chat.Id,
                ChatPartnerName = firstUser.UserName == userName ? chat.SecondUser.Name : chat.FirstUser.Name,
                ChatPartnerSurname = firstUser.UserName == userName ? chat.SecondUser.Surname : chat.FirstUser.Surname,
                ChatPartnerImageUrl = firstUser.UserName == userName ? chat.SecondUser.ImageUrl : chat.FirstUser.ImageUrl,
                ChatPartnerUserName = firstUser.UserName == userName ? chat.SecondUser.UserName : chat.FirstUser.UserName,
                Messages = _mapper.Map<IEnumerable<MessageGetDto>>(chat.Messages),
                ConnectionId = chat.ConnectionId,
            };
        }

        public async Task<ChatDeleteGetDto> DeleteMessageAsync(int id, string userName)
        {
            Message? message = await _messageRepository.GetByIdAsync(id, isTracking:true, expressionIncludes: m => m.Chat.Messages.OrderByDescending(m => m.CreatedAt).Take(20), includes:new[] { "Chat", "Chat.FirstUser", "Chat.SecondUser" });
            if (message is null) throw new NotFoundException($"Message with id {id} wasnt found!");
            if (message.Sender != userName) throw new DontHavePermissionException("You cant delete this message!");
            IList<Message> chatMessages = message.Chat.Messages;

            Message? lastMessage = chatMessages.FirstOrDefault();
            _messageRepository.Delete(message);
            if (lastMessage?.Id == message.Id )
            {
                if (message.Chat.Messages.Count > 1)
                {
                    message.Chat.LastMessageSendedAt = chatMessages[1].CreatedAt;
                    message.Chat.LastMessageSendedBy = chatMessages[1].Sender;
                    message.Chat.LastMessage = chatMessages[1].Text;
                    message.Chat.LastMessageIsChecked = chatMessages[1].IsChecked;
                }
                else
                {
                    message.Chat.LastMessageSendedAt =  null;
                    message.Chat.LastMessageSendedBy = null;
                    message.Chat.LastMessage = null;
                    message.Chat.LastMessageIsChecked = false;

                }
            }
            bool isChecked = message.IsChecked;
            await _messageRepository.SaveChangesAsync();
            return new ChatDeleteGetDto
            {
                Id = message.Chat.Id,
                FirstUserUserName = message.Chat.FirstUser.UserName,
                SecondUserUserName = message.Chat.SecondUser.UserName,
                Messages = _mapper.Map<IEnumerable<MessageGetDto>>(message.Chat.Messages),
                ConnectionId = message.Chat.ConnectionId,
                IsDeletedMessageChecked = isChecked
            };

        }
        public async Task<IEnumerable<MessageGetDto>> GetMessagesAsync(int chatId,string userName, int skip)
        {
            Chat? chat = await _chatRepository.GetByIdAsync(chatId, expressionIncludes: c => c.Messages.OrderByDescending(m => m.CreatedAt).Skip(skip).Take(20), includes: new[] { "FirstUser", "SecondUser","Messages.Media" });
            if (chat is null || chat.FirstUser.UserName != userName && chat.SecondUser.UserName != userName)
                throw new DontHavePermissionException("You cant get this chat!");
            return _mapper.Map<IEnumerable<MessageGetDto>>(chat.Messages);
        }


        public async Task<int> GetNewMessagesCountAsync(string userName)
        {
            ICollection<Chat> chats = await _chatRepository.GetCollection(c => c.FirstUser.UserName == userName 
            || c.SecondUser.UserName == userName,includes:new[] { "FirstUser", "SecondUser", "Messages" });
            int count = 0;
            foreach (Chat chat in chats)
            {
                count += chat.Messages.Where(m => m.IsChecked == false && m.Sender != userName).Count();
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
            //if (dto.Text is null && dto.Media is null) throw new MessageValidationException("Message must have at least text or media!");
            AppUser user = await _userManager.FindByNameAsync(dto.Sender);
            if (user is null) throw new AppUserNotFoundException($"User with username {dto.Sender} doesnt exists!");
            Chat? chat = await _chatRepository.GetByIdAsync(dto.ChatId,true, includes:new[] { "FirstUser", "SecondUser" });
            if (chat is null) throw new NotFoundException($"Chat with id {dto.ChatId} doesnt exists!");
            if (chat.FirstUser.UserName != dto.Sender && chat.SecondUser.UserName != dto.Sender)
                throw new DontHavePermissionException("You cant write message to this chat!");
            Message message = new Message
            {
                Text = dto.Text,
                Sender = dto.Sender,
                ChatId = chat.Id,
                CreatedAt = DateTime.UtcNow
                
            };
            //bool exceptionCatched = false;
            //bool isSucceedUpload = false;
            //if (dto.Media is not null)
            //{
            //    foreach (MediaPostDto media in dto.Media)
            //    {
            //        try
            //        {
                        
            //            FileType type = _fileService.GetFileType(media.FileName);
            //            string localUrl = await _fileService.CreateFileFromBytesAsync(media.MediaInBytes, media.FileName, "uploads", "chats");
            //            string realUrl = await _cloudinaryService.UploadFileAsync(localUrl, type, "uploads", "chats");
            //            message.Media.Add(new MessageMedia { MediaUrl = realUrl, MediaType = type });
            //            if (!isSucceedUpload) isSucceedUpload = true;
            //        }
            //        catch (BaseException ex)
            //        {
            //            exceptionCatched = true;
            //        }
            //    }
            //}
            //if (dto.Media is not null && !isSucceedUpload && dto.Text is null) throw new MessageValidationException("It seems that the media you uploaded is invalid. Please ensure the file format is either an image or video, and its size doesn't exceed 100MB.");
            //if (exceptionCatched)
            //{
            //    Notification notification = new Notification
            //    {
            //        Title = "Oops!",
            //        Text = "Some media were not sent because they did not pass validation.",
            //        Type = NotificationType.System,
            //        AppUser = user,
            //    };
            //    await _notificationRepository.CreateAsync(notification);
            //    await _notificationRepository.SaveChangesAsync();
            //    NotificationsGetDto notificationDto = new NotificationsGetDto {
            //        Title = notification.Title,
            //        Text = notification.Text,
            //        Type = notification.Type.ToString(),
            //        Id = notification.Id,
            //        CreatedAt = notification.CreatedAt,
            //        IsChecked = notification.IsChecked

            //    };
            //    await _hubContext.Clients.Group(dto.Sender).SendAsync("NewNotification", notificationDto);
            //}

            //chat.LastMessageIsMedia = isSucceedUpload;
            chat.LastMessage = dto.Text;
            chat.LastMessageSendedAt = DateTime.UtcNow;
            chat.LastMessageSendedBy = dto.Sender;
            chat.LastMessageIsChecked = false;
            await _messageRepository.CreateAsync(message);
            await _messageRepository.SaveChangesAsync();
            return _mapper.Map<MessageGetDto>(message);
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
                Sender = dto.Sender,
                Text = dto.Text,
            };
            chat.LastMessage = newMessage.Text;
            chat.LastMessageSendedAt = DateTime.UtcNow;
            chat.LastMessageSendedBy = dto.Sender;
            chat.LastMessageIsChecked = false;
            if (isNew) await _chatRepository.CreateAsync(chat);

            await _messageRepository.CreateAsync(newMessage);
            await _chatRepository.SaveChangesAsync();
            MessageGetDto messageDto = new MessageGetDto { CreatedAt = newMessage.CreatedAt, Id = newMessage.Id, Text = newMessage.Text, Sender = newMessage.Sender };
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
            if (!await _userManager.Users.AnyAsync(u => u.UserName == userName)) throw new AppUserNotFoundException($"User with username {userName} doesnt exists!");
            ICollection<Chat> userChats = await _chatRepository.OrderAndGet(
                order: c => c.LastMessageSendedAt,
                isDescending: true,
                expression: c => c.FirstUser.UserName == userName
                || c.SecondUser.UserName == userName,
                expressionIncludes: c => c.Messages.Where(m => !m.IsChecked && m.Sender != userName),
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

        public async Task<int> GetUnreadedMessagesCountAsync(string userName)
        {
            if (!await _userManager.Users.AnyAsync(u => u.UserName == userName))
                throw new AppUserNotFoundException($"User with username {userName} doesnt exists!");
            int count = await _messageRepository.GetCountAsync(
                m => !m.IsChecked && m.Sender != userName &&  (m.Chat.FirstUser.UserName == userName || m.Chat.SecondUser.UserName == userName),
                includes:new[] { "Chat", "Chat.FirstUser", "Chat.SecondUser" });
            return count;
                

        }
    }
}
