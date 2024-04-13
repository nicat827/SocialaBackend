using Microsoft.AspNetCore.SignalR;
using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Hubs
{
    public interface IMessagesHub : IClientProxy
    {
        Task CheckChatAfterSendMessage(string chatConnectionId, int chatId, string sender, MessageGetDto sendedMessage);
        
    }
}
