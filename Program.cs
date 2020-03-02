using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using WaterMyPlant.Data;
using WaterMyPlant.Data.DatabseContext;
using WaterMyPlant.Services;

namespace WaterMyPlant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            using (var scope = host.Services.CreateScope())
            {
                logger.Debug("Creating Service Scope..");
                var services = scope.ServiceProvider;

                try
                {
                    var hc = services.GetRequiredService<HangfireDbContext>();
                    var pc = services.GetRequiredService<WaterMyPlantDbContext>();
                    ContextInitializer.Initialize(hc, pc).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
      .UseNLog(); 

    }
}
