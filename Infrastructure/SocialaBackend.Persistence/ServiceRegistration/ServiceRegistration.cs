﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging;
using ProniaOnion.Persistence.DAL;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Persistence.DAL;
using SocialaBackend.Persistence.Implementations.Hubs;
using SocialaBackend.Persistence.Implementations.Repositories;
using SocialaBackend.Persistence.Implementations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialaBackend.Application.Abstractions.Hubs;

namespace SocialaBackend.Persistence.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Default")));
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = true;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                opt.Lockout.AllowedForNewUsers = true;

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

            services.AddHttpContextAccessor();

            //repos
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IStoryItemsRepository, StoryItemsRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<IFollowerRepository, FollowerRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IVerifyRequestRepository, VerifyRequestRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupMessageRepository, GroupMessageRepository>();
            //services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IManageService, ManageService>();
            services.AddScoped<IStoryService, StoryService>();
            services.AddScoped<AppDbContextInitializer>();

        }

        public static void AddPersistenceConfigure(this IApplicationBuilder app)
        {
            
        }
    }
}
