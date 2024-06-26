﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]

    public enum FileType
    {
        Text,
        Audio,
        Image,
        Video
    }
}
