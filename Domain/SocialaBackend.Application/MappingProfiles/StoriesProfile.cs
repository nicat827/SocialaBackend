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
    public class StoriesProfile :Profile
    {
        public StoriesProfile()
        {
            CreateMap<StoryItem, StoryItemCurrentGetDto>();
            CreateMap<StoryItemWatcher, StoryItemWatcherDto>();
            CreateMap<StoryItem, StoryItemGetDto>();
        }
    }
}
