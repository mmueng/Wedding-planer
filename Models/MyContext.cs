using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner.Models {
    public class MyContext : DbContext {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext (DbContextOptions options) : base (options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Wedding> Wedding { get; set; }
        public DbSet<Association> association { get; set; }
    }
}