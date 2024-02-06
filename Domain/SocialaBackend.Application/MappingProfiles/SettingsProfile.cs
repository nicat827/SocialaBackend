using AutoMapper;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.MappingProfiles
{
    public class SettingsProfile:Profile
    {
        public SettingsProfile()
        {
            CreateMap<SettingsDescriptionPostDto, AppUser>();
            CreateMap<AppUser, SettingsDescriptionGetDto>();
        }
    }
}
