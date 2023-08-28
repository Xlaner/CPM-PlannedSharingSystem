using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Web.Apis.LinkedinApi.ImageAndText;
using System.Data.Entity.Core.Common.CommandTrees;



using Microsoft.Identity.Client;
using Portal.Application.Repositories;
using Portal.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using ServiceStack;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using System.Reflection;
using static System.Net.WebRequestMethods;
using System.IO;
using System.Security.Claims;

using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

using BackGroundService;
using Portal.Web.Apis.LinkedinApi.OnlyText;

namespace BackGroundService

{
    public class MyBackgroundService : BackgroundService
    {
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IAccessTokenReadRepository _accessTokenReadRepository;


        public MyBackgroundService(
            ILogger<MyBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            // Scoped servisleri burada Initialize edin
            using (var scope = _serviceProvider.CreateScope())
            {
                _accessTokenReadRepository = scope.ServiceProvider.GetRequiredService<IAccessTokenReadRepository>();
            }
        }

        // Diğer kodlar...

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Initialize(); // Initialize metodu ile servisleri alın
            while (!stoppingToken.IsCancellationRequested)
            {
                var optionsBuilder = new DbContextOptionsBuilder<PortalDbContext>();
                optionsBuilder.UseSqlServer("Server=10.10.10.60;Database=PortalDb;User Id=sa;Password=123456;TrustServerCertificate=True;");

                using (var dbContext = new PortalDbContext(optionsBuilder.Options))
                {
                    var DbEtkinlikler = dbContext.etkinliks.ToList();
                    foreach(var Dbe in DbEtkinlikler)
                    {
                        
                        if(Dbe.start.Date == DateTime.Now.Date)
                        {
                            var DbApiler = dbContext.AccessTokens.ToList();
                            foreach(var Dba in DbApiler)
                            {

                                if(Dba.ApiTuru == "Linkedin" && Dbe.ApiId == Dba.Id.ToString())
                                {       
                                    if(Dbe.image != null)
                                    {
                                        await MainProgramImageLinkedin.RunLinkedInImageShareAsync(Dba.Token.ToString(), Dbe.description, Dbe.imagePath);

                                    }
                                    else
                                    {
                                        await MainProgramOnlyTextLinkedin.RunLinkedInOnlyTextShareAsync(Dba.Token, Dbe.description);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("eşleşme yok");
                                }

                            }
                        }
                    }
                    
                }
                // _accessTokenReadRepository'i kullanarak işlemleri gerçekleştirin
                
                
                // Burada arka planda yapılacak işlemleri gerçekleştirin.

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Örnek olarak 1 dakika bekleyin.
            }
        }
    }
}
