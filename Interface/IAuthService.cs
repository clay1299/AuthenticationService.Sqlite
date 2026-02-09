using AuthenticationService.Sqlite.Model;

namespace AuthenticationService.Sqlite.Interface;
public interface IAuthService
{
    bool LoginUser(string userName, string password);

    bool LoginUser(string userName, string password, out bool isAdmin);

    bool LoginAdmin(string userName, string password);

    bool RegisterUser(string userName, string password);

    bool UserExists(string userName);

    bool DeleteUser(string userName);

    /// <summary>
    /// Returns all persons, including admins
    /// </summary>
    List<Person> GetAll();

    /// <summary>
    /// Remember person
    /// </summary>
    /// <param name="userName">login</param>
    void RememberMe(string userName);

    /// <summary>
    /// Returns the remembered person
    /// </summary>
    IAuthEntity? GetRememberUser();

    /// <summary>
    /// Delete RememberMe File
    /// </summary>
    void ClearRememberMe();
}
