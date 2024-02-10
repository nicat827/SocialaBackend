﻿using Microsoft.AspNetCore.SignalR;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _service;

        public NotificationHub(INotificationService service)
        {
            _service = service;
        }
        public async Task Connect(string userName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, userName);
            IEnumerable<NotificationsGetDto> notifications = await _service.GetLastNotifications(userName);
            await Clients.Client(connectionId).SendAsync("LatestNotifications", notifications);
        }


        public async Task Disconnect(string userName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.RemoveFromGroupAsync(connectionId, userName);
        }

        public async Task SendLikeNotification(string userName)
        {
            await Clients.Group(userName).SendAsync("NewNotification","liked");
        }


    }
}