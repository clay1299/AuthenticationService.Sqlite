# AuthenticationService.Sqlite

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Описание

Простая, гибкая и легковесная библиотека для добавления аутентификации и авторизации пользователей с использованием SQLite в .NET приложениях. Работает как с Dependency Injection, так и без неё. Хэширует пароли с помощью [BCrypt](https://github.com/BcryptNet/bcrypt.net).

## Быстрый старт

### С Dependency Injection (Рекомендуется)

App.xaml.cs
```csharp
using AuthenticationService.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;
using System.Windows;

namespace TestApp;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        Batteries.Init();
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddAuthService("Data Source=auth.db");
        services.AddSingleton<MainWindow>();

        Services = services.BuildServiceProvider();

        //авто создание базы данных
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
            context.Database.EnsureCreated();
        }

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
```
MainWindow.xaml.cs
```csharp
using AuthenticationService.Sqlite;
using System.Windows;

namespace TestApp;
public partial class MainWindow : Window
{
    private readonly IAuthService _authService;
    public MainWindow(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    private void login_click(object sender, RoutedEventArgs e)
    {
        var login = tbLogin.Text;
        var pass = tbPassword.Text;
        bool result = _authService.LoginUser(login, pass);
        if (result)
            MessageBox.Show("Success!");
        else
            MessageBox.Show("Fail");
    }

    private void register_click(object sender, RoutedEventArgs e)
    {
        var login = tbLogin.Text;
        var pass = tbPassword.Text;
        bool result = _authService.RegisterUser(login, pass);
        if (result)
            MessageBox.Show("Success!");
        else
            MessageBox.Show("User already exists");
    }
}
```


