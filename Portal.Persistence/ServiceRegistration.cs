using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Persistence.Repositories;


namespace ControlPortal.Persistence
{
    public static class ServiceRegistiration
    {

        public static void AddPersistenceServices(this IServiceCollection services)
        {

            services.AddDbContext<PortalDbContext>(options => options.UseSqlServer("Server=10.10.10.60;Database=PortalDb;User Id=sa;Password=123456;TrustServerCertificate=True;"));

            services.AddScoped<IEtkinlikReadRepository, EtkinlikReadRepository>();
            services.AddScoped<IEtkinlikWriteRepository, EtkinlikWriteRepository>();

            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();

            services.AddScoped<IAccessTokenWriteRepository, AccessTokenWriteRepository>();
            services.AddScoped<IAccessTokenReadRepository, AccessTokenReadRepository>();

            services.AddScoped<TimerManager>();


        }

    }
}
