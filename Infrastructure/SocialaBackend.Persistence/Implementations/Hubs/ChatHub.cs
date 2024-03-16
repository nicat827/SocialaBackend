using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Hubs;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Base;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.Implementations.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Hubs
{
    public class ChatHub:Hub
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGroupMessageRepository _groupMessageRepository;
        private readonly IGroupService _groupService;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IChatService _chatService;

        private static Dictionary<string, ICollection<(string,int)>> Chats = new Dictionary<string, ICollection<(string,int)>>();
        private static Dictionary<string, ICollection<(string,int)>> _groups = new Dictionary<string, ICollection<(string,int)>>();
        public ChatHub(INotificationRepository notificationRepository, IHubContext<NotificationHub> notificationHub, UserManager<AppUser> userManager, IGroupMessageRepository groupMessageRepository, IGroupService groupService, IGroupRepository groupRepository, IMessageRepository messageRepository, IChatRepository chatRepository, IChatService chatService)
        {
            _notificationRepository = notificationRepository;
            _notificationHub = notificationHub;
            _userManager = userManager;
            _groupMessageRepository = groupMessageRepository;
            _groupService = groupService;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _chatService = chatService;
        }
        public async Task ConnectToChatSockets(string userName)
        {
            ICollection<ChatItemGetDto> userChatItems = await _chatService.GetChatItemsAsync(userName);
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            //if (!GroupCount.ContainsKey(_currentUserName)) GroupCount[_currentUserName] = 1;
            //else GroupCount[_currentUserName] = GroupCount[_currentUserName] + 1;
            int count = await _groupRepository.GetCountAsync(g => g.Members.Any(m => m.AppUser.UserName == userName), "Members", "Members.AppUser");
            await Clients.Client(Context.ConnectionId).SendAsync("GetChatItems", userChatItems);
            await Clients.Client(Context.ConnectionId).SendAsync("GetGroupsCount", count);

        }
        public async Task ConnectToGroupSockets(string userName)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(Context.ConnectionId, " " , userName);
            Console.ResetColor();
            IEnumerable<GroupItemGetDto> userGroupItems = await _groupService.GetGroupItemsAsync(userName);
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            //if (!GroupCount.ContainsKey(_currentUserName)) GroupCount[_currentUserName] = 1;
            //else GroupCount[_currentUserName] = GroupCount[_currentUserName] + 1;
            int count = await _chatRepository.GetCountAsync(c => c.FirstUser.UserName == userName || c.SecondUser.UserName == userName, "SecondUser", "FirstUser");

            await Clients.Client(Context.ConnectionId).SendAsync("GetGroupItems", userGroupItems);
            await Clients.Client(Context.ConnectionId).SendAsync("GetChatsCount", count);

        }


        public async Task Disconnect(string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName);
        }

        public async Task DisconnectChat(int chatId, string userName)
        {
            try
            {
                ChatGetDto chat = await _chatService.GetChatByIdAsync(chatId, userName);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.ConnectionId);
                if (Chats.ContainsKey(chat.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInChat = Chats[chat.ConnectionId];
                    (string, int) findedTuple = usersInChat.FirstOrDefault(tuple => tuple.userName == userName);
                    if (findedTuple.Item1 is not null)
                    {
                        if (findedTuple.Item2 <= 1) usersInChat.Remove(findedTuple);
                        else findedTuple.Item2--;
                    }
                }
            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex.Message}");
            }
        }

        public async Task DisconnectGroup(int groupId, string userName)
        {
            try
            {
                GroupGetDto group = await _groupService.GetGroupByIdAsync(groupId, userName);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.ConnectionId);
                if (_groups.ContainsKey(group.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInChat = _groups[group.ConnectionId];
                    (string, int) findedTuple = usersInChat.FirstOrDefault(tuple => tuple.userName == userName);
                    if (findedTuple.Item1 is not null)
                    {
                        if (findedTuple.Item2 <= 1) usersInChat.Remove(findedTuple);
                        else findedTuple.Item2--;
                    }
                }
            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex.Message}");
            }
        }
        public async Task AddTypingUser(string reciever, string sender)
        {
            
            await Clients.Group(reciever).SendAsync("GetAddedTypingUser", sender);
        }

        public async Task AddGroupTypingUser(int groupId, string sender)
        {
            GroupGetDto group = await _groupService.GetGroupByIdAsync(groupId, sender);
            foreach (string member in group.Members.Select(m => m.UserName))  
            {
                if (member != sender)
                {
                    await Clients.Group(member).SendAsync("GetGroupAddedTypingUser", groupId, sender);
                }
            }
        }
        public async Task DeleteTypingUser(string reciever, string sender)
        {

            await Clients.Group(reciever).SendAsync("GetDeletedTypingUser", sender);
        }
        public async Task DeleteGroupTypingUser(int groupId, string sender)
        {
            GroupGetDto group = await _groupService.GetGroupByIdAsync(groupId, sender);
            foreach (string member in group.Members.Select(m => m.UserName))
            {
                if (member != sender)
                {
                    await Clients.Group(member).SendAsync("GetGroupDeletedTypingUser", groupId, sender);
                }
            }
        }
        public async Task GetItems(bool isChat, string userName)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine(Context.ConnectionId + " " + userName);
                Console.ResetColor();
                if (isChat) await Clients.Client(Context.ConnectionId).SendAsync("GetChatItems", await _chatService.GetChatItemsAsync(userName));
                else await Clients.Client(Context.ConnectionId).SendAsync("GetGroupItems", await _groupService.GetGroupItemsAsync(userName));
            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex.Message}");
            }
        }
        public async Task ConnectToChat(int chatId, string userName)
        {
            ChatGetDto chat = await _chatService.GetChatByIdAsync(chatId, userName);
            var lastMess = chat.Messages.FirstOrDefault();
            if (lastMess is not null)
            {
                if (!lastMess.IsChecked && lastMess.Sender != userName)
                {
                    Chat chatFromDb = await _chatRepository.GetByIdAsync(chatId, true);
                    chatFromDb.LastMessageIsChecked = true;
                    await _chatRepository.SaveChangesAsync();

                }
            }
            int count = 0;
            foreach (MessageGetDto message in chat.Messages)
            {
                if (message.IsChecked == false && message.Sender != userName)
                {
                    count++;
                    message.IsChecked = true;
                    
                    Message messFromDb = await _messageRepository.GetByIdAsync(message.Id, true);
                    messFromDb.IsChecked = true;
                    await _messageRepository.SaveChangesAsync();
                }
            }
            if (count > 19)
            {
                ICollection<Message> allUnreadedMessages = await _messageRepository.GetAllUnreadedMessagesAsync(chatId);
                foreach (Message mess in allUnreadedMessages)
                {
                    if (mess.Sender != userName) 
                    {
                        mess.IsChecked = true;
                        await _messageRepository.SaveChangesAsync();
                    }
                }

            }
            ICollection<ChatItemGetDto> userChatItems = await _chatService.GetChatItemsAsync(userName);
            ICollection<ChatItemGetDto> partnerChatItems = await _chatService.GetChatItemsAsync(chat.ChatPartnerUserName);

            await Clients.Group(userName).SendAsync("GetChatItems", userChatItems);
            await Clients.Group(chat.ChatPartnerUserName).SendAsync("GetChatItems", partnerChatItems);

            await Groups.AddToGroupAsync(Context.ConnectionId, chat.ConnectionId);
            if (!Chats.ContainsKey(chat.ConnectionId))
            {
                Chats[chat.ConnectionId] = new List<(string, int)> {(userName, 1)};
            }
            else
            {
                ICollection<(string userName, int count)> usersInChat = Chats[chat.ConnectionId];
                (string, int) findedTuple = usersInChat.FirstOrDefault(tuple => tuple.userName == userName);
                if (findedTuple.Item1 is null) usersInChat.Add((userName, 1));
                else findedTuple.Item2++;
            }

            await Clients.Client(Context.ConnectionId).SendAsync("ChatConnectResponse", chat);
        }
        public async Task ConnectToGroup(int groupId, string userName)
        {
            GroupGetDto group = await _groupService.GetGroupByIdAsync(groupId, userName);
            var lastMess = group.Messages.FirstOrDefault();
            if (lastMess is not null)
            {
                if (!lastMess.IsChecked && lastMess.Sender != userName)
                {
                    Group groupFromDb = await _groupRepository.GetByIdAsync(groupId, true);
                    groupFromDb.LastMessageIsChecked = true;
                    await _chatRepository.SaveChangesAsync();

                }
            }
            int count = 0;
            AppUser? checkedUser = await _userManager.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
            foreach (GroupMessageGetDto message in group.Messages)
            {
                if (message.Sender != userName && !message.CheckedUsers.Any(u => u.UserName == userName))
                {
                    //BURA DUSURSE DEMELI GORMUYUB INDI CONNECT OLUNAN ISTIFADECI
                    count++;
                    if (!message.IsChecked)
                    {

                        message.IsChecked = true;
                    }
                    count++;
                    GroupMessage messFromDb = await _groupMessageRepository.GetByIdAsync(message.Id,isTracking: true, includes:"CheckedUsers");
                    if (checkedUser is not null)
                    {
                        messFromDb.CheckedUsers.Add(new GroupMessageWatcher { AppUser = checkedUser});
                        _groupMessageRepository.Update(messFromDb);
                        await _groupMessageRepository.SaveChangesAsync();
                    }
                }
            }
            if (count> 19)
            {
                ICollection<GroupMessage> allUnreadedMessages = await _groupMessageRepository.GetAllUnreadedGroupMessagesAsync(groupId, userName);
                AppUser? connectedUser = await _userManager.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
                foreach (GroupMessage message in allUnreadedMessages)
                {
                    if (checkedUser is not null)
                    {
                        message.CheckedUsers.Add(new GroupMessageWatcher { AppUser = checkedUser });
                        _groupMessageRepository.Update(message);
                        await _groupMessageRepository.SaveChangesAsync();
                    }
                }
            }
            
            foreach (string member  in group.Members.Select(m => m.UserName)) await Clients.Group(member).SendAsync("GetGroupItems", await _groupService.GetGroupItemsAsync(member));
            await Groups.AddToGroupAsync(Context.ConnectionId, group.ConnectionId);
            if (!_groups.ContainsKey(group.ConnectionId))
            {
                _groups[group.ConnectionId] = new List<(string, int)> { (userName, 1) };
            }
            else
            {
                ICollection<(string userName, int count)> usersInChat = _groups[group.ConnectionId];
                (string, int) findedTuple = usersInChat.FirstOrDefault(tuple => tuple.userName == userName);
                if (findedTuple.Item1 is null) usersInChat.Add((userName, 1));
                else findedTuple.Item2++;
            }

            await Clients.Client(Context.ConnectionId).SendAsync("GroupConnectResponse", group);
        }
        public async Task DeleteMessage(int id, string userName)
        {
            if (id <= 0) return;
            ChatDeleteGetDto dto = await _chatService.DeleteMessageAsync(id, userName);
            ICollection<ChatItemGetDto> firstChatItems = await _chatService.GetChatItemsAsync(dto.FirstUserUserName);
            ICollection<ChatItemGetDto> secondChatItems = await _chatService.GetChatItemsAsync(dto.SecondUserUserName);

            await Clients.Group(dto.ConnectionId).SendAsync("GetMessagesAfterDelete", dto.Messages);
            await Clients.Group(dto.FirstUserUserName).SendAsync("GetChatItems", firstChatItems);
            await Clients.Group(dto.SecondUserUserName).SendAsync("GetChatItems", secondChatItems);
        }
        public async Task AddGroupAdmin(int groupId, string userName, string addedBy)
        {
            if (groupId <= 0) return;
            Group? group = await _groupRepository.GetByIdAsync(groupId, true, includes: new[] { "Members", "Members.AppUser" });
            if (group is null) throw new NotFoundException($"Group with id {groupId} wasnt found!");
            GroupMemberItem? itemToAdd = group.Members.FirstOrDefault(m => m.AppUser.UserName == userName);
            if (itemToAdd is null) throw new NotFoundException($"User with username {userName} doesnt exits in group!");
            GroupMemberItem? item = group.Members.FirstOrDefault(m => m.AppUser.UserName == addedBy);
            if (item is null) throw new NotFoundException($"User with username {addedBy} doesnt exits in group!");
            if (itemToAdd.Role == GroupRole.Member && item.Role != GroupRole.Member) itemToAdd.Role = GroupRole.Admin;

            Notification newNotification = new Notification
            {
                AppUser = itemToAdd.AppUser,
                Title = "New Role!",
                Type = NotificationType.Chat,
                Text = $"You have been added to the admins group {group.Name} by {addedBy}!"
            };
            await _notificationRepository.CreateAsync(newNotification);
            await _notificationRepository.SaveChangesAsync();
            NotificationsGetDto notifyDto = new NotificationsGetDto
            {
                CreatedAt = newNotification.CreatedAt,
                Id = newNotification.Id,
                Text = newNotification.Text,
                Title = newNotification.Title,
                Type = newNotification.Type.ToString(),
                UserName = userName
            };
            await _groupRepository.SaveChangesAsync();
            await _notificationHub.Clients.Group(userName).SendAsync("NewNotification", notifyDto);
            var dto = new List<GroupMembersGetDto>();
            foreach (GroupMemberItem groupMemberItem in group.Members)
            {
                dto.Add(new GroupMembersGetDto
                {
                    Id = groupMemberItem.Id,
                    Name = groupMemberItem.AppUser.Name,
                    Surname = groupMemberItem.AppUser.Surname,
                    GroupRole = groupMemberItem.Role.ToString(),
                    UserName = groupMemberItem.AppUser.UserName,
                    ImageUrl = groupMemberItem.AppUser.ImageUrl
                });
            }
            foreach (var member in group.Members) await Clients.Group(member.AppUser.UserName).SendAsync("GetGroupMembersAfterDelete", dto);



        }
        public async Task RemoveMemberFromGroup(int groupId, string removeableUserName, string removedBy)
        {
            if (groupId <= 0) return;
            try
            {
                Group? group = await _groupRepository.GetByIdAsync(groupId, true, includes: new[] { "Members", "Members.AppUser" });
                if (group is null) throw new NotFoundException($"Group with id {groupId} wasnt found!");
                GroupMemberItem? removeableItem = group.Members.FirstOrDefault(m => m.AppUser.UserName == removeableUserName);
                if (removeableItem is null) throw new NotFoundException($"User with username {removeableUserName} doesnt exits in group!");
                GroupMemberItem? item = group.Members.FirstOrDefault(m => m.AppUser.UserName == removedBy);
                if (item is null) throw new NotFoundException($"User with username {removedBy} doesnt exits in group!");

                if (removeableItem.Role == GroupRole.Member && item.Role != GroupRole.Member) group.Members.Remove(removeableItem);
                else if (removeableItem.Role == GroupRole.Admin && item.Role == GroupRole.Owner) group.Members.Remove(removeableItem);
                else throw new DontHavePermissionException($"You cant delete user {removeableUserName} from group!");
                Notification newNotification = new Notification
                {
                    AppUser = removeableItem.AppUser,
                    Title = "Kicked!",
                    Type = NotificationType.Chat,
                    Text = $"You have been kicked from group {group.Name} by {removedBy}!"
                };
                await _notificationRepository.CreateAsync(newNotification);
                await _notificationRepository.SaveChangesAsync();
                NotificationsGetDto notifyDto = new NotificationsGetDto
                {
                    CreatedAt = newNotification.CreatedAt,
                    Id = newNotification.Id,
                    Text = newNotification.Text,
                    Title = newNotification.Title,
                    Type = newNotification.Type.ToString(),
                    UserName = removeableUserName
                };
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.ConnectionId);
                if (_groups.ContainsKey(group.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInChat = _groups[group.ConnectionId];
                    (string, int) findedTuple = usersInChat.FirstOrDefault(tuple => tuple.userName == removeableUserName);
                    if (findedTuple.Item1 is not null)
                    {
                        if (findedTuple.Item2 <= 1) usersInChat.Remove(findedTuple);
                        else findedTuple.Item2--;
                    }
                }
                await _groupRepository.SaveChangesAsync();
                ICollection<GroupItemGetDto> removeableGroupItems = await _groupService.GetGroupItemsAsync(removeableUserName);
                await Clients.Group(removeableUserName).SendAsync("GetGroupItems", removeableGroupItems);
                await Clients.Group(removeableUserName).SendAsync("GetRemovedGroupId", group.Id);
                await _notificationHub.Clients.Group(removeableUserName).SendAsync("NewNotification", notifyDto);
                var dto = new List<GroupMembersGetDto>();
                foreach (GroupMemberItem groupMemberItem in group.Members)
                {
                    dto.Add(new GroupMembersGetDto
                    {
                        Id = groupMemberItem.Id,
                        Name = groupMemberItem.AppUser.Name,
                        Surname = groupMemberItem.AppUser.Surname,
                        GroupRole = groupMemberItem.Role.ToString(),
                        UserName = groupMemberItem.AppUser.UserName,
                        ImageUrl = groupMemberItem.AppUser.ImageUrl
                    });
                }
                foreach (var member in group.Members) await Clients.Group(member.AppUser.UserName).SendAsync("GetGroupMembersAfterDelete", dto);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }

        }
        public async Task DeleteGroupMessage(int id, string userName)
        {
            if (id <= 0) return;
            try
            {
                await _groupService.DeleteMessageAsync(id, userName);

            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", ex.Message);
            }
        }
        public async Task SendMessageByChatId(MessagePostDto dto)
        {
            try
            {
                ChatGetDto chat = await _chatService.GetChatByIdAsync(dto.ChatId, dto.Sender);
                MessageGetDto sendedMessage = await _chatService.SendMessageAsync(dto);
                if (Chats.ContainsKey(chat.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInCurrentChat = Chats[chat.ConnectionId];
                    if (usersInCurrentChat.Any(tuple => tuple.userName != dto.Sender))
                    {
                        sendedMessage.IsChecked = true;
                        Message messFromDb = await _messageRepository.GetByIdAsync(sendedMessage.Id, true);
                        messFromDb.IsChecked = true;
                        Chat chatFromDb = await _chatRepository.GetByIdAsync(dto.ChatId, true);
                        chatFromDb.LastMessageIsChecked = true;
                        await _messageRepository.SaveChangesAsync();
                    }
                }
                ICollection<ChatItemGetDto> userChatItems = await _chatService.GetChatItemsAsync(dto.Sender);
                ICollection<ChatItemGetDto> partnerChatItems = await _chatService.GetChatItemsAsync(chat.ChatPartnerUserName);

                await Clients.Group(chat.ConnectionId).SendAsync("RecieveMessage", sendedMessage);
                await Clients.Group(dto.Sender).SendAsync("GetChatItems", userChatItems);
                await Clients.Group(chat.ChatPartnerUserName).SendAsync("GetChatItems", partnerChatItems);
            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex}");
            }
        }

        public async Task SendMessageToGroup(GroupMessagePostDto dto)
        {
            try
            {
                GroupGetDto group = await _groupService.GetGroupByIdAsync(dto.GroupId, dto.Sender);
                GroupMessageGetDto sendedMessage = await _groupService.SendMessageAsync(dto);
                if (_groups.ContainsKey(group.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInCurrentGroup = _groups[group.ConnectionId];
                    usersInCurrentGroup = usersInCurrentGroup.Where(tuple => tuple.userName !=dto.Sender).ToList();
                    if (usersInCurrentGroup?.Count > 0)
                    {
                        sendedMessage.IsChecked = true;
                        Group groupFromDb = await _groupRepository.GetByIdAsync(group.Id, true);
                        groupFromDb.LastMessageIsChecked = true;
                        await _groupRepository.SaveChangesAsync();
                        GroupMessage messFromDb = await _groupMessageRepository.GetByIdAsync(sendedMessage.Id, true, includes:"CheckedUsers");
                        foreach (var user in usersInCurrentGroup)
                        {
                            AppUser checkedUser = await _userManager.FindByNameAsync(user.userName);
                            messFromDb.CheckedUsers.Add(new GroupMessageWatcher { AppUser = checkedUser });
                            _groupMessageRepository.Update(messFromDb);
                            await _groupMessageRepository.SaveChangesAsync();
                        }
                    }
                }
                foreach (string member in group.Members.Select(m => m.UserName)) await Clients.Group(member).SendAsync("GetGroupItems", await _groupService.GetGroupItemsAsync(member));

                await Clients.Group(group.ConnectionId).SendAsync("RecieveGroupMessage", sendedMessage);
            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex.Message}");
            }
        }

        public async Task GetChatMessages(int chatId, string userName, int skip)
        {
            if (skip < 0) await Clients.Client(Context.ConnectionId).SendAsync("RecieveChatMessages", new List<MessageGetDto>());
            try
            {
                IEnumerable<MessageGetDto> messages = await _chatService.GetMessagesAsync(chatId, userName, skip);
                await Clients.Client(Context.ConnectionId).SendAsync("RecieveChatMessages", messages);
            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", ex.Message);
            }
        }
        public async Task GetGroupMessages(int groupId, string userName, int skip)
        {
            if (skip < 0) await Clients.Client(Context.ConnectionId).SendAsync("RecieveGroupMessages", new List<MessageGetDto>());
            try
            {
                IEnumerable<GroupMessageGetDto> messages = await _groupService.GetMessagesAsync(groupId, userName, skip);
                await Clients.Client(Context.ConnectionId).SendAsync("RecieveGroupMessages", messages);
            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", ex.Message);
            }
        }
        public async Task SearchChatUsers(string searchParam, string userName)
        {
            try
            {
                ICollection<ChatItemSearchGetDto> searchedUsers = await _chatService.SearchChatUsersAsync(searchParam, userName);
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine(searchParam);
                Console.ResetColor();
                await Clients.Client(Context.ConnectionId).SendAsync("GetSearchedUsers", searchedUsers);
            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex.Message}");
            }
        }
        public async Task SendMessageByUserName(MessagePostDtoFromProfile dto)
        {
            try
            {
                
                (MessageGetDto sendedMessage, ChatGetDto chat) = await _chatService.SendMessageFromProfileAsync(dto);
                if (Chats.ContainsKey(chat.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInCurrentChat = Chats[chat.ConnectionId];
                    if (usersInCurrentChat.Any(tuple => tuple.userName != dto.Sender))
                    {
                        sendedMessage.IsChecked = true;
                        Message messFromDb = await _messageRepository.GetByIdAsync(sendedMessage.Id, true);
                        messFromDb.IsChecked = true;
                        Chat chatFromDb = await _chatRepository.GetByIdAsync(chat.Id, true);
                        chatFromDb.LastMessageIsChecked = true;
                        await _messageRepository.SaveChangesAsync();
                    }
                }
                ICollection<ChatItemGetDto> userChatItems = await _chatService.GetChatItemsAsync(dto.Sender);
                ICollection<ChatItemGetDto> partnerChatItems = await _chatService.GetChatItemsAsync(chat.ChatPartnerUserName);

                await Clients.Group(chat.ConnectionId).SendAsync("RecieveMessage", sendedMessage);
                await Clients.Group(dto.Sender).SendAsync("GetChatItems", userChatItems);
                await Clients.Group(chat.ChatPartnerUserName).SendAsync("GetChatItems", partnerChatItems);


            }
            catch (BaseException ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SendMessageError", $"{ex.Message}");
            }
        }

        //groups logic

       
    }
}
