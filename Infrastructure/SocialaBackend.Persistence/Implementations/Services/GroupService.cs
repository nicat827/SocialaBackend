using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.Implementations.Hubs;
using SocialaBackend.Persistence.Implementations.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class GroupService:IGroupService
    {
        private readonly ILogger<GroupService> _logger;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<MessagesHub> _messagesHub;
        private readonly IGroupMessageRepository _groupMessageRepository;
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileService _fileService;
        private readonly ICloudinaryService _cloudinaryService;

        public GroupService(ILogger<GroupService> logger, IHubContext<NotificationHub> notificationHub, INotificationRepository notificationRepository, IHubContext<MessagesHub> messagesHub, IGroupMessageRepository groupMessageRepository, IMapper mapper, IGroupRepository groupRepository, UserManager<AppUser> userManager, IFileService fileService, ICloudinaryService cloudinaryService)
        {
            _logger = logger;
            _notificationHub = notificationHub;
            _notificationRepository = notificationRepository;
            _messagesHub = messagesHub;
            _groupMessageRepository = groupMessageRepository;
            _mapper = mapper;
            _groupRepository = groupRepository;
            _userManager = userManager;
            _fileService = fileService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task CreateGroupAsync(GroupCreateDto dto) 
        {
            AppUser? owner = await _userManager.Users
                .Where(u => u.UserName == dto.OwnerUserName)
                .Include(u => u.Follows.Where(uf => uf.IsConfirmed))
                .FirstOrDefaultAsync();
            if (owner is null) throw new AppUserNotFoundException($"User with username {dto.OwnerUserName} doesnt exists!");
            
            Group newGroup = new Group
            {
                Name = dto.Name,
                Description = dto.Description,
                ConnectionId = Guid.NewGuid().ToString(),
            };
            newGroup.Members.Add(new GroupMemberItem { AppUser = owner, Role = GroupRole.Owner });
            foreach (GroupMembersPostDto member in dto.Members)
            {
                if (!newGroup.Members.Any(m => m.AppUser.UserName == member.UserName))
                {
                    FollowItem? item = owner.Follows.FirstOrDefault(fi =>  fi.UserName == member.UserName);
                    if (item is null) throw new DontHavePermissionException($"You cant create group with user {member.UserName}!");
                    AppUser itemUser = await _userManager.FindByNameAsync(member.UserName);
                    if (itemUser is null) throw new AppUserNotFoundException($"User with username {member.UserName} doesnt exists!");
                    newGroup.Members.Add(new GroupMemberItem { AppUser = itemUser, Role =  member.GroupRole.ToLower() == GroupRole.Member.ToString().ToLower() ? GroupRole.Member : GroupRole.Admin});
                }
            }
            if (dto.Photo is not null)
            {
                string imageUrl = await _fileService.CreateFileAsync(dto.Photo, "uploads", "groups");
                string cloudinaryUrl = await _cloudinaryService.UploadFileAsync(imageUrl, FileType.Image, "uploads", "groups");
                newGroup.ImageUrl = cloudinaryUrl;
            }
            await _groupRepository.CreateAsync(newGroup);
            await _groupRepository.SaveChangesAsync();
            GroupItemGetDto newGroupItem = new GroupItemGetDto
            {
                GroupId = newGroup.Id,
                Name = newGroup.Name,
                ImageUrl = newGroup.ImageUrl,
                LastMessage = newGroup.LastMessage,
                LastMessageIsChecked = newGroup.LastMessageIsChecked,
                LastMessageSendedAt = newGroup.LastMessageSendedAt,
                LastMessageSendedBy = newGroup.LastMessageSendedBy,
                UnreadedMessagesCount = 0
            };
            foreach (string member in newGroup.Members.Select(m => m.AppUser.UserName))
            {
                await _messagesHub.Clients.Group(member).SendAsync("GetNewGroup", newGroupItem);
            }
        }

        public async Task<ICollection<GroupItemGetDto>> GetGroupItemsAsync(string userName)
        {
            if (!await _userManager.Users.AnyAsync(u => u.UserName == userName)) throw new AppUserNotFoundException($"User with username {userName} doesnt exists!");
            _logger.LogInformation("Do zaprosa");
            ICollection<Group> userGroups = await _groupRepository.OrderAndGet
                (
                    order: g => g.LastMessageSendedAt,
                    isDescending: true,
                    expression: g => g.Members.Any(m => m.AppUser.UserName == userName),
                    expressionIncludes:g => g.Messages.Where(m => m.Sender != userName && !m.CheckedUsers.Any(cu => cu.AppUser.UserName == userName)),
                    includes: new[] { "Members", "Members.AppUser", "Messages.CheckedUsers", "Messages.CheckedUsers.AppUser" }
                ).ToListAsync();
            ICollection<GroupItemGetDto> dto = new List<GroupItemGetDto>();
            if (userGroups is not null)
            {
                foreach (var group in userGroups)
                {
                    _logger.LogInformation(group.Name, userGroups.Count);

                }
                _logger.LogInformation(userGroups.Count.ToString());
                foreach (Group group in userGroups)
                {
                    dto.Add(new GroupItemGetDto
                    {
                        GroupId = group.Id,
                        Name = group.Name,
                        ImageUrl = group.ImageUrl,
                        LastMessage = group.LastMessage,
                        LastMessageIsChecked = group.LastMessageIsChecked,
                        LastMessageSendedAt = group.LastMessageSendedAt,
                        LastMessageSendedBy = group.LastMessageSendedBy,
                        UnreadedMessagesCount = group.Messages.Count
                    });
                }

            }
            return dto;
        }

       

        public async Task DeleteMessageAsync(int id, string userName)
        {
            GroupMessage? message = await _groupMessageRepository.GetByIdAsync(id, isTracking: true, expressionIncludes: m => m.Group.Messages.OrderByDescending(m => m.CreatedAt).Take(20), includes: new[] { "Group", "Group.Members", "Group.Members.AppUser" });
            if (message is null) throw new NotFoundException($"Message with id {id} wasnt found!");
            GroupMemberItem currentMemberInGroup = message.Group.Members.FirstOrDefault(m => m.AppUser.UserName == userName);
            if (currentMemberInGroup == null) throw new DontHavePermissionException("You cant delete this message!");
           
            if (message.Sender != userName && currentMemberInGroup.Role == GroupRole.Member) throw new DontHavePermissionException("You cant delete this message!");
            IList<GroupMessage> groupMessages = message.Group.Messages;

            string? lastMessage = groupMessages.FirstOrDefault().Text;
            _groupMessageRepository.Delete(message);
            if (lastMessage == message.Text)
            {
                if (message.Group.Messages.Count > 1)
                {
                    message.Group.LastMessageSendedAt = groupMessages[1].CreatedAt;
                    message.Group.LastMessageSendedBy = groupMessages[1].Sender;
                    message.Group.LastMessage = groupMessages[1].Text;
                }
                else
                {
                    message.Group.LastMessageSendedAt = null;
                    message.Group.LastMessageSendedBy = null;
                    message.Group.LastMessage = null;
                }
            }
            await _groupMessageRepository.SaveChangesAsync();
            ICollection<GroupMessageGetDto> messagesDto = new List<GroupMessageGetDto>();
            foreach (GroupMessage mess in message.Group.Messages) messagesDto.Add(new GroupMessageGetDto
            {
                Id = mess.Id,
                CreatedAt = mess.CreatedAt,
                Sender = mess.Sender,
                Text = mess.Text,
                ImageUrl = mess.ImageUrl,
                IsChecked = mess.CheckedUsers.Count > 0,
            });
            await _messagesHub.Clients.Group(message.Group.ConnectionId).SendAsync("GetGroupMessagesAfterDelete",messagesDto);
            foreach (var member in message.Group.Members)
            {
                ICollection<GroupItemGetDto> dto = await GetGroupItemsAsync(member.AppUser.UserName);
                await _messagesHub.Clients.Group(member.AppUser.UserName).SendAsync("GetGroupItems", dto);
            }



        }

        public async Task<IEnumerable<GroupMessageGetDto>> GetMessagesAsync(int chatId, string userName, int skip)
        {
            Group? group = await _groupRepository.GetByIdAsync(chatId, expressionIncludes: c => c.Messages.OrderByDescending(m => m.CreatedAt).Skip(skip).Take(20), includes: new[] { "Messages.CheckedUsers", "Messages.CheckedUsers.AppUser", "Members", "Members.AppUser" });
            if (group is null || !group.Members.Any(m => m.AppUser.UserName == userName))
                throw new DontHavePermissionException("You cant get this chat!");
            ICollection<GroupMessageGetDto> dto = new List<GroupMessageGetDto>();
            foreach (GroupMessage mess in group.Messages)
            {
                dto.Add(new GroupMessageGetDto
                {
                    Id = mess.Id,
                    CreatedAt = mess.CreatedAt,
                    Sender = mess.Sender,
                    Text = mess.Text,
                    ImageUrl = mess.ImageUrl,
                    IsChecked = mess.CheckedUsers.Count > 0,
                });
            }
            return dto;
        }

        public async Task<GroupGetDto> GetGroupByIdAsync(int id, string userName)
        {
            Group? group = await _groupRepository.GetByIdAsync(id, 
                expressionIncludes: g => g.Messages.OrderByDescending(gm => gm.CreatedAt).Take(20),
                includes:new[] { "Messages.CheckedUsers", "Messages.CheckedUsers.AppUser", "Members", "Members.AppUser"});
            if (group is null) throw new NotFoundException($"Group with id {id} didnt found!");
            if (!group.Members.Any(m => m.AppUser.UserName == userName)) throw new DontHavePermission("You cant get this group!");

            GroupGetDto dto = new GroupGetDto
            {
                Id = group.Id,
                ConnectionId = group.ConnectionId,
                Name = group.Name,
                ImageUrl = group.ImageUrl,
                Description = group.Description,
                UsersCount = group.Members.Count,
            };
           
            foreach (GroupMessage message in group.Messages)
            {
                var item = new GroupMessageGetDto
                {
                    Id = message.Id,
                    CreatedAt = message.CreatedAt,
                    Sender = message.Sender,
                    Text = message.Text,
                    ImageUrl = message.ImageUrl,
                    IsChecked = message.CheckedUsers.Count > 0,
                };
                foreach (GroupMessageWatcher checkedUser in message.CheckedUsers)
                {
                    item.CheckedUsers.Add(new CheckedUserGetDto { ImageUrl = checkedUser.AppUser.ImageUrl, UserName = checkedUser.AppUser.UserName });
                }
                dto.Messages.Add(item);
            }
            foreach (GroupMemberItem groupMemberItem in group.Members)
            {
                dto.Members.Add(new GroupMembersGetDto
                {
                    Id = groupMemberItem.Id,
                    Name = groupMemberItem.AppUser.Name,
                    Surname = groupMemberItem.AppUser.Surname,
                    GroupRole = groupMemberItem.Role.ToString(),
                    UserName = groupMemberItem.AppUser.UserName,
                    ImageUrl = groupMemberItem.AppUser.ImageUrl
                });
            }
            return (dto);
        }

        public async Task<GroupMessageGetDto> SendMessageAsync(GroupMessagePostDto dto)
        {
            AppUser user = await _userManager.FindByNameAsync(dto.Sender);
            if (user is null) throw new AppUserNotFoundException($"User with username {dto.Sender} doesnt exists!");
            Group? group = await _groupRepository.GetByIdAsync(dto.GroupId, true, includes: new[] { "Members", "Members.AppUser"});
            if (group is null) throw new NotFoundException($"Group with id {dto.GroupId} doesnt exists!");
            if (!group.Members.Any(m => m.AppUser.UserName == dto.Sender)) throw new DontHavePermission("You cant get this group!");

            GroupMessage message = new GroupMessage
            {
                Text = dto.Text,
                Sender = dto.Sender,
                GroupId = group.Id,
                ImageUrl = dto.ImageUrl
            };
            group.LastMessage = dto.Text;
            group.LastMessageSendedAt = DateTime.UtcNow;
            group.LastMessageSendedBy = dto.Sender;
            group.LastMessageIsChecked = false;
            await _groupMessageRepository.CreateAsync(message);
            await _groupMessageRepository.SaveChangesAsync();
            return new GroupMessageGetDto
            {
                Id = message.Id,
                Sender = message.Sender,
                IsChecked = message.IsDeleted,
                Text = message.Text,
                CreatedAt = message.CreatedAt,
                ImageUrl = message.ImageUrl
            };
        }

        public async Task<int> GetGroupsCountAsync(string userName)
        {
            return await _groupRepository.GetCountAsync(g => g.Members.Any(m => m.AppUser.UserName == userName), "Members", "Members.AppUser");
        }

    }
}
