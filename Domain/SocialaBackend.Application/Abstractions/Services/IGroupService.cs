using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IGroupService
    {
        Task CreateGroupAsync(GroupCreateDto dto);
        Task<ICollection<GroupItemGetDto>> GetGroupItemsAsync(string userName);
        Task<IEnumerable<GroupMessageGetDto>> GetMessagesAsync(int chatId, string userName, int skip);
        Task DeleteMessageAsync(int id, string userName);
        Task<GroupGetDto> GetGroupByIdAsync(int id, string userName);
        Task<GroupMessageGetDto> SendMessageAsync(GroupMessagePostDto dto);
    }
}
