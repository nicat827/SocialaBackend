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
    internal class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Message>> GetAllUnreadedMessagesAsync(int chatId)
        {
            return await _context.Messages.Include(m => m.Chat).Where(m => m.ChatId == chatId && !m.IsChecked).ToListAsync();
        }
    }
}
