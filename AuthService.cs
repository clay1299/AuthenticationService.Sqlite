namespace AuthenticationService.Sqlite;
internal class AuthService : IAuthService
{
    private readonly AuthContext _context;
    private readonly string _connectionString;

    public AuthService(string connectionString)
    {
        _connectionString = connectionString;
        _context = CreateContext();
    }

    internal AuthService(AuthContext context)
    {
        _context = context;
    }

    private AuthContext CreateContext()
    {
        return _context ?? new AuthContext(_connectionString);
    }


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
}
