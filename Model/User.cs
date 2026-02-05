using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Sqlite.Model;

public class User : IAuthEntity
{
    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}