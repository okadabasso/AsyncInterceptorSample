using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AsyncInterceptorSample.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        public void ClearEntryState()
        {
            var entries = ChangeTracker.Entries().ToArray();
            foreach (var entry in entries)
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

        }
        public DbSet<Foo> Foos { get; set; }
    }
}
