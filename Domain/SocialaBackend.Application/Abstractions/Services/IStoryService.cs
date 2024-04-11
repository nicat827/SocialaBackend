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
        Task<IEnumerable<StoryGetDto>> GetStoriesAsync();
        Task<ICollection<StoryItemGetDto>> GetStoryItemsAsync(int storyId);
        Task<IEnumerable<StoryItemCurrentGetDto>> GetCurrentUserStoryItemsAsync();
        Task SoftRemoveStoryItemAsync(int id);
        Task WatchStoryItemAsync(int id);
        Task<IEnumerable<StoryItemWatcherDto>> GetStoryItemWatchersAsync(int id);
        Task<IEnumerable<StoryItemGetDto>> GetArchivedStoryItemsAsync(int skip);

    }
}
