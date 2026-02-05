using AuthenticationService.Sqlite.Interface;
using AuthenticationService.Sqlite.Model;

namespace AuthenticationService.Sqlite;
public class AdminRegister : IAdminRegister
{
    private readonly AuthContext _context;

    public AdminRegister(AuthContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public bool RegisterAdmin(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            return false;

        if (_context.Admins.Any(u => u.UserName == userName))
            return false;

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        _context.Admins.Add(new Admin
        {
            UserName = userName,
            Password = hashedPassword
        });

        _context.SaveChanges();
        return true;
    }

    public bool DeleteAdmin(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return false;

        var admin = _context.Admins.FirstOrDefault(u => u.UserName == userName);

        if (admin == null)
            return false;

        _context.Admins.Remove(admin);
        _context.SaveChanges();
        return true;
    }

}
