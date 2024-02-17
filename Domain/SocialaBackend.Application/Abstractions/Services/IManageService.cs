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

        Task AddToRoleUserAsync(string userName, UserRole role);

        Task AddToRolesUserAsync(string userName, IEnumerable<UserRole> userRoles);

        Task RemoveFromRoleUserAsync(string userName, UserRole role);

        Task<ManageGetDto> GetManageAsync();
    }
}
