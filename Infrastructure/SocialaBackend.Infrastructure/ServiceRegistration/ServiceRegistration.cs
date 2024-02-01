using Microsoft.Extensions.DependencyInjection;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Infrastructure.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Infrastructure.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IFIleService, FileService>();
        }
    }
}
