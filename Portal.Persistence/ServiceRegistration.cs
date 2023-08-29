using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Persistence.Repositories;
using BackGroundService;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ControlPortal.Persistence
{
    public static class ServiceRegistiration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {

            var currentDirectory = Directory.GetCurrentDirectory();
            var jsonFilePath = Path.Combine(currentDirectory, "appsettings.json");// JSON dosyasının yolunu belirtin
            using (StreamReader file = File.OpenText(jsonFilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JToken jToken = JToken.ReadFrom(reader);

                // JSON içeriği üzerinde işlemler yapabilirsiniz
                var connectionStrings = jToken["ConnectionStrings"];
                var sqlServerConnectionString = connectionStrings["SqlServer"].ToString();

                services.AddDbContext<PortalDbContext>(options => options.UseSqlServer(sqlServerConnectionString));

                services.AddScoped<IEtkinlikReadRepository, EtkinlikReadRepository>();
                services.AddScoped<IEtkinlikWriteRepository, EtkinlikWriteRepository>();

                services.AddScoped<IUserReadRepository, UserReadRepository>();
                services.AddScoped<IUserWriteRepository, UserWriteRepository>();

                services.AddScoped<IAccessTokenWriteRepository, AccessTokenWriteRepository>();
                services.AddScoped<IAccessTokenReadRepository, AccessTokenReadRepository>();

                services.AddSingleton<MyBackgroundService>();
                services.AddHostedService(provider => provider.GetRequiredService<MyBackgroundService>());

            }
        }
    }
}
