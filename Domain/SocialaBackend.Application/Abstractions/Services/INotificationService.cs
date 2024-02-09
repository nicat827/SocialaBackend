using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationsGetDto>> GetLastNotifications(string userName);
    }
}
