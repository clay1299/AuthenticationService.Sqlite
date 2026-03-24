using AuthenticationService.Sqlite.Interface;
using AuthenticationService.Sqlite.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Sqlite;
public class AuthService : IAuthService
{
    private readonly AuthContext _context;
    private const string _remeberMePath = "remember.dat";
    private Person _currentUser;

    public AuthService(AuthContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Person CurrentUser
    {
        get => _currentUser;
        private set
        {
            if(_currentUser != value)
                _currentUser = value;
        }
    }

    public void RemoveCurrentUser() => CurrentUser = null;

    public bool LoginUser(string userName, string password)
    {        
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
            return false;

        if (BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            CurrentUser = new Person
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = Roles.User
            };
            return true;
        }
        return false;
    }

    public bool LoginUser(string userName, string password, out bool isAdmin)
    {
        isAdmin = false;

        var admin = _context.Admins.FirstOrDefault(u => u.UserName == userName);
        if (admin != null)
        {
            if (BCrypt.Net.BCrypt.Verify(password, admin.Password))
            {
                isAdmin = true;
                CurrentUser = new Person
                {
                    Id = admin.Id,
                    UserName = admin.UserName,
                    Role = Roles.Admin
                };
                return true;
            }
            return false;
        }

        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user != null)
        {
            if(BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                CurrentUser = new Person
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = Roles.User
                };
                return true;
            }
        }

        return false;
    }

    public bool LoginAdmin(string userName, string password)
    {
        var admin = _context.Admins.FirstOrDefault(u => u.UserName == userName);
        if (admin == null)
            return false;

        if(BCrypt.Net.BCrypt.Verify(password, admin.Password))
        {
            CurrentUser = new Person
            {
                Id = admin.Id,
                UserName = admin.UserName,
                Role = Roles.Admin
            };
            return true;
        }
        return false;
    }

    public bool RegisterUser(string userName, string password)
    {
        if (_context.Users.Any(u => u.UserName == userName))
            return false;

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var newUser = new User
        {
            UserName = userName,
            Password = hashedPassword
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        CurrentUser = new Person
        {
            Id = newUser.Id,
            UserName = newUser.UserName,
            Role = Roles.User
        };

        return true;
    }

    public bool UserExists(string userName) => _context.Users.Any(u => u.UserName == userName);

    public List<Person> GetAll()
    {
        var users = _context.Users
            .AsNoTracking() 
            .Select(u => new Person
            {
                Id = u.Id,
                UserName = u.UserName,
                Role = Roles.User
            });

        var admins = _context.Admins
            .AsNoTracking()
            .Select(a => new Person
            {
                Id = a.Id,
                UserName = a.UserName,
                Role = Roles.Admin
            });

        return users.Concat(admins).ToList();
    }


    public bool DeleteUser(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }

    public void RememberMe(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        var admin = _context.Admins.FirstOrDefault(u => u.UserName == userName);

        if (user == null && admin == null)
            throw new Exception($"Incorrect userName: {userName}");

        string dataToStore = admin != null
            ? $"Admin:{admin.Id}"
            : $"User:{user!.Id}";

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _remeberMePath);
        if (File.Exists(path))
        {
            File.SetAttributes(path, FileAttributes.Normal);
        }

        byte[] data = Encoding.UTF8.GetBytes(dataToStore);
        byte[] enc = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

        File.WriteAllBytes(path, enc);
        File.SetAttributes(path, FileAttributes.Hidden | FileAttributes.NotContentIndexed);
    }

    public IAuthEntity? GetRememberUser()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _remeberMePath);

        if (!File.Exists(path)) return null;

        try
        {
            byte[] enc = File.ReadAllBytes(path);
            byte[] dec = ProtectedData.Unprotect(enc, null, DataProtectionScope.CurrentUser);
            string storedString = Encoding.UTF8.GetString(dec);

            var parts = storedString.Split(':');
            if (parts.Length != 2) return null;

            string role = parts[0];
            if (!int.TryParse(parts[1], out int id)) return null;

            IAuthEntity? entity = role switch
            {
                "Admin" => (IAuthEntity?)_context.Admins.FirstOrDefault(a => a.Id == id),
                "User" => (IAuthEntity?)_context.Users.FirstOrDefault(u => u.Id == id),
                _ => null
            };

            if(entity != null)
            {
                CurrentUser = new Person
                {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    Role = role == "Admin" ? Roles.Admin : Roles.User
                };
            }
            return entity;
        }
        catch
        {
            return null;
        }
    }

    public void ClearRememberMe()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _remeberMePath);
        if (File.Exists(path))
            File.Delete(path);
    }
}
