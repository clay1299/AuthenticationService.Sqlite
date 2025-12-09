using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationService.Sqlite;
public class AuthService : IAuthService
{
    private readonly AuthContext _context;

    internal AuthService(AuthContext context)
    {
        _context = context;
    }

    public IServiceProvider Services { get; private set; }

    public bool LoginUser(string userName, string password)
    {        
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
            return false;

        bool verifed = BCrypt.Net.BCrypt.Verify(password, user.Password);
        return verifed;
        
    }

    public bool RegisterUser(string userName, string password)
    {
        if (_context.Users.Any(u => u.UserName == userName))
            return false;

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        _context.Users.Add(new User
        {
            UserName = userName,
            Password = hashedPassword
        });

        _context.SaveChanges();
        return true;
    }

    public bool UserExists(string userName) => _context.Users.Any(u => u.UserName == userName);

    public bool DeleteUser(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }

    
    public void EnsureDatabaseCreated()
    {
        var services = new ServiceCollection();

        Services = services.BuildServiceProvider();

        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
            context.Database.EnsureCreated();
        }
    }
}
