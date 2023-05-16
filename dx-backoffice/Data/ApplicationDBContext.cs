using Microsoft.EntityFrameworkCore;
using dx_backoffice.Models;

namespace dx_backoffice.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) :base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PirateShipModel>().Property(x => x.Id).ValueGeneratedOnAdd();
        }

        public DbSet<StateModel> State { get; set; }
        public DbSet<PirateShipModel> PirateShips { get; set; }
    }
}
