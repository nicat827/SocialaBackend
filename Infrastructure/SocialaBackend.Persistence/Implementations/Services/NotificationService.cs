using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<NotificationsGetDto>> GetLastNotifications()
        {
            IEnumerable<Notification> notificatons = await _repository.OrderAndGet(n => n.Id, true, limit: 10).ToListAsync();
            return _mapper.Map<IEnumerable<NotificationsGetDto>>(notificatons);
        }

    }
}
