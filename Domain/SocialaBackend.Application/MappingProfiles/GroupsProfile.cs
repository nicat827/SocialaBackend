using AutoMapper;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.MappingProfiles
{
    public class GroupsProfile:Profile
    {
        public GroupsProfile()
        {
            CreateMap<Group, GroupItemGetDto>().ReverseMap();
            CreateMap<Group, GroupGetDto>();
            CreateMap<GroupMessage, GroupMessageGetDto>();
            CreateMap<AppUser, CheckedUserGetDto>();
        }
    }
}
