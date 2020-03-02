using Microsoft.EntityFrameworkCore;

namespace WaterMyPlant.Data.DatabseContext
{
    public sealed class HangfireDbContext : DbContext
    {
        public HangfireDbContext(DbContextOptions<HangfireDbContext> options) : base(options)
        {
           
        }
    }
}