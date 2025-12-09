using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Sqlite
{
    public class AuthContext : DbContext
    {
        private readonly string _connectionString;

        public AuthContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }

        public DbSet<User> Users { get; set; }
    }
}
