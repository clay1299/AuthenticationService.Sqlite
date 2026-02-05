using AuthenticationService.Sqlite.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationService.Sqlite;
public static class AuthServiceExtensions
{
    public static IServiceCollection AddAuthService(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuthContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IAuthService>(sp =>
        {
            var context = sp.GetRequiredService<AuthContext>();
            return new AuthService(context);
        });

        services.AddScoped<IAdminRegister>(sp =>
        {
            var context = sp.GetRequiredService<AuthContext>();
            return new AdminRegister(context);
        });

        return services;
    }
}
