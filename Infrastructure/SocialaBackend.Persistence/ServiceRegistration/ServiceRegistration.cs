using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProniaOnion.Persistence.DAL;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Persistence.DAL;
using SocialaBackend.Persistence.Implementations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                opt.User.RequireUniqueEmail = false;
                
                opt.Lockout.AllowedForNewUsers = true;

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

            //services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AppDbContextInitializer>();
        }
    }
}
