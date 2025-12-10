namespace AuthenticationService.Sqlite;
public interface IAuthService
{
    bool LoginUser(string userName, string password);
    bool RegisterUser(string userName, string password);
    bool UserExists(string userName);
    bool DeleteUser(string userName);

    /// <summary>
    /// Remember user
    /// </summary>
    /// <param name="userName"></param>
    void RememverMe(string userName);

    /// <summary>
    /// Returns the remembered user
    /// </summary>
    User GetRememberUser();

    /// <summary>
    /// Delete RememberMe File
    /// </summary>
    void ClearRememberMe();
}
