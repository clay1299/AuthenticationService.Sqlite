# AuthenticationService.Sqlite

[![NuGet](https://img.shields.io/nuget/v/AuthenticationService.Sqlite.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/AuthenticationService.Sqlite/)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Установка

#### Package Manager
```powershell
Install-Package AuthenticationService.Sqlite
```

#### .NET CLI
```bash
dotnet add package AuthenticationService.Sqlite
```

## Описание

Простая, гибкая и легковесная библиотека для добавления аутентификации, авторизации и безопасного RememberMe-функционала в .NET/WPF приложениях. Работает с Dependency Injection. Использует SQLite и встроенный безопасный механизм защиты данных Windows DPAPI. Пароли хэшируются с помощью [BCrypt](https://github.com/BcryptNet/bcrypt.net).

## Функциональность

- 🔐 **Аутентификация** - проверка логина/пароля
- 👥 **Регистрация** - создание новых пользователей
- 🎭 **Роли** - роли админа и пользователя
- 🔒 **Хэширование паролей** - с использованием BCrypt
- 💾 **SQLite хранение** - легковесная база данных
- 🏗️ **Интеграция с DI** - готовность к использованию в ASP.NET Core и приложениях с DI
- 📦 **Минимальные зависимости** - только необходимые пакеты
- ⭐ Безопасный Remember Me


## Быстрый старт

### Подключение через Dependency Injection

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
        Batteries.Init(); // Инициализация SQLite - обязательно!
        base.OnStartup(e);

        var services = new ServiceCollection();

        // рекомендую использовать именно такую версию. Что бы бд случайно не удалилась пользователем.
        services.AddAuthService($"Data Source={DirectoryHelper.GetAppDataPath("TestApp", "auth.db")}"); 

        services.AddSingleton<MainWindow>();

        Services = services.BuildServiceProvider();

        //Автоматическое создание БД
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

### RememberMe

Сохранение пользователя после успешного входа

```csharp
private void login_click(object sender, RoutedEventArgs e)
{
    var login = tbLogin.Text;
    var pass = tbPassword.Password;

    bool result = _authService.LoginUser(login, pass);

    if (result)
    {
        if (cbRememberMe.IsChecked == true)
            _authService.RememverMe(login);

        MessageBox.Show("Success!");
    }
    else
    {
        MessageBox.Show("Fail");
    }
}
```

Автоматический вход при старте приложения

```csharp
public MainWindow(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        var person = _authService.GetRememberUser();
        if(person is User user)
        {
            MessageBox.Show($"Добро пожаловать {person.UserName}");
        }
        else if(person is Admin admin)
        {
            MessageBox.Show($"Добро пожаловать, Админ {person.UserName}");
        }
    }
```


