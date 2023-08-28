using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Web.Apis.LinkedinApi.ImageAndText;
using System.Data.Entity.Core.Common.CommandTrees;

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
                                Console.WriteLine(Dba.Id);
                                Console.WriteLine(Dbe.ApiId);
                                if(Dbe.ApiId == Dba.Id.ToString())
                                {
                                     
                                    await MainProgramImageLinkedin.RunLinkedInImageShareAsync(Dba.Token.ToString(), Dbe.description, Dbe.imagePath);
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
