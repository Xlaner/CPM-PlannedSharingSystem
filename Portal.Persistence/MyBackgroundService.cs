using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Web.Apis.LinkedinApi.ImageAndText;

using Portal.Web.Apis.LinkedinApi.OnlyText;

namespace BackGroundService

{
    public class MyBackgroundService : BackgroundService
    {
        private readonly ILogger<MyBackgroundService> _logger;




        public MyBackgroundService(
            ILogger<MyBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;

        }


        // Diğer kodlar...

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

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
                                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                                    }
                                    else
                                    {
                                        await MainProgramOnlyTextLinkedin.RunLinkedInOnlyTextShareAsync(Dba.Token.ToString(), Dbe.description);
                                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Etkinlik Paylaşımı Yapılamadı");
                                }

                            }
                        }
                    }
                    
                }
                // _accessTokenReadRepository'i kullanarak işlemleri gerçekleştirin
                
                
                // Burada arka planda yapılacak işlemleri gerçekleştirin.

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Örnek olarak 1 dakika bekleyin.
            }
        }
    }
}
