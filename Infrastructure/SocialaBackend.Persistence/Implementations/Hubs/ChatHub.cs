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
        private readonly IChatService _chatService;

        private static Dictionary<string, ICollection<(string,int)>> Chats = new Dictionary<string, ICollection<(string,int)>>();
        public ChatHub(IMessageRepository messageRepository, IChatService chatService)
        {
            _messageRepository = messageRepository;
            _chatService = chatService;
        }
        public async Task Connect(string userName)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(userName + " USERNAMEEEEEE");
            Console.ResetColor();
            ICollection<ChatItemGetDto> userChatItems = await _chatService.GetChatItemsAsync(userName);
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            //if (!GroupCount.ContainsKey(_currentUserName)) GroupCount[_currentUserName] = 1;
            //else GroupCount[_currentUserName] = GroupCount[_currentUserName] + 1;
           
            await Clients.Client(Context.ConnectionId).SendAsync("GetChatItems", userChatItems);

        }

        public async Task Disconnect(string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName);
          
        }

        public async Task DisconnectChat(int chatId, string userName)
        {
            try
            {
                ChatGetDto chat = await _chatService.GetChatByIdAsync(chatId);
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

        public async Task ConnectToChat(int chatId, string userName)
        {
            ChatGetDto chat = await _chatService.GetChatByIdAsync(chatId);
            foreach (MessageGetDto message in chat.Messages)
            {
                if (message.IsChecked == false && message.Sender != userName)
                {
                    message.IsChecked = true;
                    Message messFromDb = await _messageRepository.GetByIdAsync(message.Id);
                    messFromDb.IsChecked = true;
                    await _messageRepository.SaveChangesAsync();
                }
            }

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
            await Clients.Client(Context.ConnectionId).SendAsync("ChatConnect", chat);
        }

        public async Task SendMessageByChatId(MessagePostDto dto)
        {
            try
            {
                ChatGetDto chat = await _chatService.GetChatByIdAsync(dto.ChatId);
                MessageGetDto sendedMessage = await _chatService.SendMessageAsync(dto);
                if (Chats.ContainsKey(chat.ConnectionId))
                {
                    ICollection<(string userName, int count)> usersInCurrentChat = Chats[chat.ConnectionId];
                    if (usersInCurrentChat.Any(tuple => tuple.userName != dto.Sender))
                    {
                        sendedMessage.IsChecked = true;
                        Message messFromDb = await _messageRepository.GetByIdAsync(sendedMessage.Id);
                        messFromDb.IsChecked = true;
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
                        Message messFromDb = await _messageRepository.GetByIdAsync(sendedMessage.Id);
                        messFromDb.IsChecked = true;
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
