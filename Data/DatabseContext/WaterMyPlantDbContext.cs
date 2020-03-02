using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterMyPlant.Models.Entity;

namespace WaterMyPlant.Data.DatabseContext
{
    public class WaterMyPlantDbContext : IdentityDbContext<IdentityUser>
    {
        public WaterMyPlantDbContext(DbContextOptions<WaterMyPlantDbContext> options) : base( options)
        {
            
        }
        
        public DbSet<PlantModel> Plants { get; set; }
        public DbSet<WateringPlantJobModel> WateringPlantJobs { get; set; }
        public DbSet<ThirstyPlantAlertModel> ThirstyPlantAlerts { get; set; }

    }
}