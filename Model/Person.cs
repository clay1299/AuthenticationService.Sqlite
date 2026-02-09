namespace AuthenticationService.Sqlite.Model;
public class Person
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public Roles Role { get; set; }
}


public enum Roles
{
    User,
    Admin
}