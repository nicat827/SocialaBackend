using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProniaOnion.Persistence.DAL
{
    public class AppDbContextInitializer
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AppDbContextInitializer(

            AppDbContext context,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task InitializeDbAsync()
        {
            await _context.Database.MigrateAsync();
        }
        public async Task CreateRolesAsync()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = role.ToString()
                });

                }
            }
        }
        public async Task CreateAdminAsync()
        {
            if (!await _userManager.Users.AnyAsync(u => u.UserName == _configuration["AdminSettings:UserName"]))
            {
                AppUser user = new AppUser
                {
                    Name = "Super",
                    Surname = "Admin",
                    UserName = _configuration["AdminSettings:UserName"],
                    Email = _configuration["AdminSettings:Email"],
                    EmailConfirmed = true
               
                };
                var res  = await _userManager.CreateAsync(user, _configuration["AdminSettings:Password"]);
                if (!res.Succeeded)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var err in res.Errors)
                    {
                        sb.AppendLine(err.ToString());
                    }
                    Console.WriteLine(sb.ToString());
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
                    Story story = new Story { Owner = user };
                    await _context.Stories.AddAsync(story);
                    await _context.SaveChangesAsync();

                }

            }
        }
    }
}
