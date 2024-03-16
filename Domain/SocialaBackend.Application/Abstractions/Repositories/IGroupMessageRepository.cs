using SocialaBackend.Application.Abstractions.Repositories.Generic;
using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Repositories
{
    public interface IGroupMessageRepository:IGenericRepository<GroupMessage>
    {
        Task<ICollection<GroupMessage>> GetAllUnreadedGroupMessagesAsync(int groupId, string userName);
    }
}
