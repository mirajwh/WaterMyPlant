using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WaterMyPlant.Data;
using WaterMyPlant.Data.DatabseContext;
using WaterMyPlant.Misc;
using WaterMyPlant.Services;

namespace WaterMyPlant
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
                
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HangfireDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("HangfireDbConnection")));
            services.AddDbContext<WaterMyPlantDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("WaterMyPlantDbConnection")));
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IWateringService, WateringService>();
            services.AddTransient<IPlantService, PlantService>();

            // HangFire Scheduler
            services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(Configuration.GetConnectionString("HangfireDbConnection"));

                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WaterMango API", Version = "v1" });
            });

            services.AddControllersWithViews();
            
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSignalR();


        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPlantService plantService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WaterMango API V1");

            });

            

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire");

            InsertDummyData.Insert(plantService).GetAwaiter().GetResult();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PlantHub>("/plantHub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
