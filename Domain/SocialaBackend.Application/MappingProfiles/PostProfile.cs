using AutoMapper;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.MappingProfiles
{
    public class PostProfile:Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostGetDto>();
        }
    }
}
