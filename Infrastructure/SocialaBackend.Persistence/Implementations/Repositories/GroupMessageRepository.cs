using Microsoft.EntityFrameworkCore;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Persistence.DAL;
using SocialaBackend.Persistence.Implementations.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Repositories
{
    internal class GroupMessageRepository : GenericRepository<GroupMessage>, IGroupMessageRepository
    {
        private readonly AppDbContext _context;

        public GroupMessageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<GroupMessage>> GetAllUnreadedGroupMessagesAsync(int groupId, string userName)
        {
            return await _context.GroupMessages
                .Include(m => m.Group)
                .Include(m => m.CheckedUsers)
                    .ThenInclude(cu => cu.AppUser)
                .Where(gm => gm.GroupId == groupId && !gm.CheckedUsers.Any(cu => cu.AppUser.UserName ==userName))
                .ToListAsync();
        }
    }
}
