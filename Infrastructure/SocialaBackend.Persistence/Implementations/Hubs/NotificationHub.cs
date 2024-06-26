﻿using Microsoft.AspNetCore.SignalR;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Persistence.Implementations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialaBackend.Persistence.Implementations.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _service;
        private  static IList<string> ConnectedUsers = new List<string>();
        private  static Dictionary<string, int> GroupCount = new Dictionary<string, int>();
        private  static Dictionary<string, string> DictionaryConnect = new Dictionary<string, string>();
        public NotificationHub(INotificationService service)
        {
            _service = service;
        }

    
        public async Task Connect(string userName)
        {
            var connectionId = Context.ConnectionId;
            DictionaryConnect[connectionId] = userName;
            await Groups.AddToGroupAsync(connectionId, userName);
            if (!ConnectedUsers.Any(i => i == userName)) ConnectedUsers.Add(userName);
            if (!GroupCount.ContainsKey(userName))
            {
                GroupCount[userName] = 1;
                await Clients.All.SendAsync("OnlineUsers", ConnectedUsers);
               
            }
            else
            {
                GroupCount[userName] = GroupCount[userName] + 1;
                await Clients.Client(connectionId).SendAsync("OnlineUsers", ConnectedUsers);

            }
            IEnumerable<NotificationsGetDto> notifications = await _service.GetLastNotifications(userName);
            await Clients.Client(connectionId).SendAsync("LatestNotifications", notifications);
          
        }
   
        //public async Task Disconnect(string userName)
        //{
        //    var connectionId = Context.ConnectionId;
        //    await Groups.RemoveFromGroupAsync(connectionId, userName);
        //    GroupCount[userName] = GroupCount[userName] - 1;
        //    if (GroupCount[userName] == 0)
        //    {
        //        ConnectedUsers.Remove(userName);
        //        GroupCount.Remove(userName);
        //        await Clients.All.SendAsync("OnlineUsers", ConnectedUsers);
        //    }
            
        //}

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
           
            if (DictionaryConnect.TryGetValue(Context.ConnectionId, out var userName))
            {
                
                DictionaryConnect.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName);
                GroupCount[userName] = GroupCount[userName] - 1;
                if (GroupCount[userName] == 0)
                {
                    ConnectedUsers.Remove(userName);
                    GroupCount.Remove(userName);
                    await Clients.All.SendAsync("OnlineUsers", ConnectedUsers);
                }
            }
        }

        public async Task SendLikeNotification(string userName)
        {
            await Clients.Group(userName).SendAsync("NewNotification","liked");
        }


    }
}
