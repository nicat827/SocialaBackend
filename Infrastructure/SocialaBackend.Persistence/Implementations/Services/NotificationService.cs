using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
        public async Task<IEnumerable<NotificationsGetDto>> GetLastNotifications(string userName)
        {
            IEnumerable<Notification> notificatons = await _repository.OrderAndGet(n => n.Id, true, n => n.AppUser.UserName == userName, limit: 10,includes:"AppUser").ToListAsync();

            ICollection<NotificationsGetDto> dto = new List<NotificationsGetDto>();
            foreach (Notification notification in  notificatons)
            {
                dto.Add(new NotificationsGetDto
                {
                    Id = notification.Id,
                    CreatedAt = notification.CreatedAt,
                    Text = notification.Text,
                    Title = notification.Title,
                    IsChecked = notification.IsChecked,
                    UserName = notification.UserName,
                    SrcId = notification.SrcId,
                    Type = notification.Type.ToString(),
                    SourceUrl = notification.SourceUrl
                });
            }
            return dto;
        }

    }
}
