using System;
using System.Threading.Tasks;
using Hangfire;
using WaterMyPlant.Data.DatabseContext;
using WaterMyPlant.Misc;
using WaterMyPlant.Services;

namespace WaterMyPlant.Data
{
    public static  class ContextInitializer
    {
        public static async Task Initialize(HangfireDbContext hc, WaterMyPlantDbContext pc)
        {
            await hc.Database.EnsureCreatedAsync();
            await pc.Database.EnsureCreatedAsync();
        }       
    }
}