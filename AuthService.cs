using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Sqlite;
public class AuthService : IAuthService
{
    private readonly AuthContext _context;
    private const string _remeberMePath = "remember.dat";

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

    public void RememverMe(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
            throw new Exception("Incorrect userName");

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _remeberMePath);

        byte[] data = Encoding.UTF8.GetBytes(user.Id.ToString());
        byte[] enc = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

        File.WriteAllBytes(path, enc);
        File.SetAttributes(path, FileAttributes.Hidden | FileAttributes.NotContentIndexed);
    }

    public User GetRememberUser()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _remeberMePath);

        if (!File.Exists(path))
            return null;

        byte[] enc = File.ReadAllBytes(path);
        byte[] dec = ProtectedData.Unprotect(enc, null, DataProtectionScope.CurrentUser);

        string idStr = Encoding.UTF8.GetString(dec);

        if (!int.TryParse(idStr, out int userId))
            return null;

        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    public void ClearRememberMe()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _remeberMePath);
        if (File.Exists(path))
            File.Delete(path);
    }
}
