using AutoMapper;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Dtos.AppUsers;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.MappingProfiles
{
    public class AppUserProfile:Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUserRegisterDto, AppUser>();
            CreateMap<AppUser, AppUserGetDto>();
            CreateMap<AppUser, CurrentAppUserGetDto>();
        }
    }
}
