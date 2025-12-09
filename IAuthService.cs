namespace AuthenticationService.Sqlite;
public interface IAuthService
{
    bool LoginUser(string userName, string password);
    bool RegisterUser(string userName, string password);
    bool UserExists(string userName);
    bool DeleteUser(string userName);
}
