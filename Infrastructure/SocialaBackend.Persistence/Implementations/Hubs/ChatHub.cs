using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.Chat;
using SocialaBackend.Application.Exceptions.Base;
using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Hubs
{
    public class ChatHub:Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IChatService _chatService;

        private static Dictionary<string, ICollection<(string,int)>> Chats = new Dictionary<string, ICollection<(string,int)>>();
        public ChatHub(IMessageRepository messageRepository, IChatRepository chatRepository, IChatService chatService)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
            _chatService = chatService;
        }
        public async Task Connect(string userName)
        {
            ICollection<ChatItemGetDto> userChatItems = await _chatService.GetChatItemsAsync(userName);
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            //if (!GroupCount.ContainsKey(_currentUserName)) GroupCount[_currentUserName] = 1;
            //else GroupCount[_currentUserName] = GroupCount[_currentUserName] + 1;
           
            await Clients.Client(Context.ConnectionId).SendAsync("GetChatItems", userChatItems);

        }

     

        public async Task Disconnect(string userName)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("DEFAULTTT");
            Console.ResetColor();

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
        public async Task AddTypingUser(string reciever, string sender)
        {
            
            await Clients.Group(reciever).SendAsync("GetAddedTypingUser", sender);
        }
        public async Task DeleteTypingUser(string reciever, string sender)
        {

            await Clients.Group(reciever).SendAsync("GetDeletedTypingUser", sender);
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
    }
}
