using SocialaBackend.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface IStoryService
    {
        Task CreateStoryItemAsync(StoryItemPostDto dto);
        Task<ICollection<StoryGetDto>> GetStoriesAsync();
        Task<ICollection<StoryItemGetDto>> GetStoryItemsAsync(int storyId);
        Task<ICollection<StoryItemCurrentGetDto>> GetCurrentUserStoryItemsAsync();

    }
}
