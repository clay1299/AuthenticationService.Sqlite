namespace AuthenticationService.Sqlite.Interface;
public interface IAdminRegister
{
    bool RegisterAdmin(string userName, string password);
    bool DeleteAdmin(string userName);
}
