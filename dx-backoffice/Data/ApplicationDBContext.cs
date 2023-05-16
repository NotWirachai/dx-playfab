using Microsoft.EntityFrameworkCore;
using dx_backoffice.Models;

namespace dx_backoffice.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) :base(options) 
        {

        }

        public DbSet<StateModel> State { get; set; }
    }
}
