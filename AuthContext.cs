using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Sqlite
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
