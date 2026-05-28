// App.xaml.cs — WPF startup file.
// Services are created here and passed where needed.
// WPF does not have a built-in DI container, so we create services manually
// and store them as static properties on App for easy access anywhere.

using HabiCheck.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace HabiCheck;

public partial class App : Application
{
    // 💡 DEVELOPER NOTE: 'static' means these can be accessed from anywhere in the app
    // without needing an App instance: App.Database, App.Habi, App.Antigravity
    public static DatabaseService Database { get; private set; } = null!;
    public static HabiService Habi { get; private set; } = null!;
    public static AntigravityService Antigravity { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Create services in dependency order (Database first, then Habi which needs Database)
        Database = new DatabaseService();
        Habi = new HabiService(Database);
        Antigravity = new AntigravityService(new HttpClient());

        // Show the login window as the first screen.
        // We look for LoginWindow in the HabiCheck.Views namespace.
        var login = new HabiCheck.Views.LoginWindow();
        login.Show();
    }
}
