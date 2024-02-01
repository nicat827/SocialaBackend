using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
            await _context.Database.EnsureCreatedAsync();
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
            AppUser user = new AppUser
            {
                Name = "admin",
                Surname = "admin",
                Email = _configuration["AdminSettings:Email"],
                UserName = _configuration["AdminSettings:UserName"],
            };
            await _userManager.CreateAsync(user, _configuration["AdminSettings:Password"]);
            await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
        }
    }
}
