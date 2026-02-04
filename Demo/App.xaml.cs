using System.Windows;
using FluentDialogs.Abstractions;
using FluentDialogs.Demo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDialogs.Demo;

/// <summary>
/// Application entry point demonstrating FluentDialogs.Wpf integration.
/// </summary>
/// <remarks>
/// This demo application shows the recommended way to integrate FluentDialogs
/// with dependency injection in a WPF application.
/// </remarks>
public partial class App : Application
{
    /// <summary>
    /// Gets the application's service provider for dependency injection.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Configures services and initializes the application.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Step 1: Configure the service collection
        var services = new ServiceCollection();
        ConfigureServices(services);

        // Step 2: Build the service provider
        Services = services.BuildServiceProvider();

        // Step 3: Initialize the theme service (optional but recommended)
        var themeService = Services.GetRequiredService<IMessageBoxThemeService>();
        if (themeService is Services.MessageBoxThemeService concreteService)
        {
            concreteService.Initialize();
        }

        // Step 4: Create and show the main window with injected ViewModel
        var mainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    /// <summary>
    /// Configures all services for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    private static void ConfigureServices(IServiceCollection services)
    {
        // Register FluentDialogs services
        // This single call registers both IMessageBoxService and IMessageBoxThemeService
        services.AddFluentDialogs();

        // Register ViewModels
        services.AddTransient<MainViewModel>();
    }
}
