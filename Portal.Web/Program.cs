using BackGroundService;
using ControlPortal.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace YourNamespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddControllersWithViews();
                services.AddPersistenceServices();


                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.Cookie.Name = "NetCoreMvcAuth";
                    options.LoginPath = "/Login/Index";
                    options.AccessDeniedPath = "/Login/Index";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                });

                services.AddHostedService<MyBackgroundService>();
            })
            .Configure(app =>
            {
                var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                if (!env.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Login}/{action=Index}/{id?}");
                });
            });
        });

    }
}
