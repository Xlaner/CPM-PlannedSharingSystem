using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portal.Application.Repositories;
using Portal.Persistence.Context;
using Portal.Web.Apis.LinkedinApi.ImageAndText;
using System;
using System.Timers;
using Timer = System.Timers.Timer;


namespace ControlPortal.Persistence
{

    public class TimerManager
    {


        private Timer myTimer;
        PortalDbContext dbContext;
        public TimerManager(DbContextOptions<PortalDbContext> dbContextOptions)
        {
            dbContext = new PortalDbContext(dbContextOptions);
            myTimer = new Timer();
            myTimer.Interval = 5000;
            myTimer.Elapsed += TimerElapsed;
            myTimer.Start();
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var Id = "deneme";
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
  




    }

}