namespace AuthenticationService.Sqlite.Model;

public interface IAuthEntity
{
    int Id { get; set; }
    string UserName { get; set; }
}
