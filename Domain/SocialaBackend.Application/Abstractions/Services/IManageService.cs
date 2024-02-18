using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IManageService
    {
        Task<ICollection<AppUserSearchDto>> SearchUsersAsync(string searchTerm, int skip);
        Task ChangeRolesUserAsync(string userName, IEnumerable<UserRole> roles);
        Task<ManageGetDto> GetManageAsync();
    }
}
