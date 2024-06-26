﻿using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Dtos
{
    public class StoryItemCurrentGetDto
    {
        public int Id { get; set; }
        public string SourceUrl { get; set; } = null!;
        public string? Text { get; set; }
        public FileType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int WatchCount { get; set; }
        public bool IsWatched { get; set; }
        public IEnumerable<StoryItemWatcherDto> Watchers { get; set; } = new List<StoryItemWatcherDto>();

    }
}
