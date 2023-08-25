using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Application.Repositories;

namespace YourNamespace
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly IEtkinlikReadRepository etkinlikReadRepository;
        public MyBackgroundService(ILogger<MyBackgroundService> logger, IEtkinlikReadRepository etkinlikReadRepository)
        {
            _logger = logger;
            this.etkinlikReadRepository = etkinlikReadRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Arka planda çalışan servis çalışıyor.");
                var datas = etkinlikReadRepository.Get();
                // Burada arka planda yapılacak işlemleri gerçekleştirin.
                foreach (var data in datas)
                {
                    Console.WriteLine(data.Id);
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Örnek olarak 1 dakika bekleyin.
            }
        }
    }
}
