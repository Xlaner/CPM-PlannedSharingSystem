using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackGroundService;

namespace Portal.Persistence
{
    public static class ConfugureServices
    {

        public static void AddConfugureServices(this IServiceCollection services)
        {



            services.AddSingleton<MyBackgroundService>();
            services.AddHostedService(provider => provider.GetRequiredService<MyBackgroundService>());



        }

    }
}
