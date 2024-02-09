using Microsoft.AspNetCore.SignalR;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Persistence.Implementations.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNotification(string message)
        {
            await _hubContext.Clients.All.SendAsync("message", message);
        }
    }
}
