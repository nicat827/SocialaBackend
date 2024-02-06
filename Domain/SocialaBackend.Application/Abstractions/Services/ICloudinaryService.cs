﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Application.Abstractions.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(string imageUrl, params string[] folders);
    }
}