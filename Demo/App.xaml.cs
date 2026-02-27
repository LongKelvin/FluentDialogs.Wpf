using System.Windows;
using System.Threading;

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
    private static Mutex? _mutex;
    private const string MutexName = "{8F6F0AC4-B9A1-4cfe-A545-E45A88F3C111}";

    /// <summary>
    /// Gets the application's service provider for dependency injection.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Configures services and initializes the application.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Check for existing instance FIRST, before base.OnStartup
        try
        {
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            
            if (!createdNew)
            {
                // Another instance is already running
                MessageBox.Show(
                    "FluentDialogs Demo is already running. Only one instance is allowed.",
                    "Already Running",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                
                // Shutdown this instance
                Shutdown();
                return;
            }
        }
        catch
        {
            // If we can't create/check the mutex, continue anyway
        }

        base.OnStartup(e);

        // Step 1: Configure the service collection
        var services = new ServiceCollection();
        ConfigureServices(services);

        // Step 2: Build the service provider
        Services = services.BuildServiceProvider();

        // Step 3: Initialize the v2 theme service — detects FluentDialogs.Theme.xaml
        // already loaded via App.xaml and injects the default preset + SyncBrushColors
        var themeService = Services.GetRequiredService<IFluentDialogThemeService>();
        themeService.EnsureThemeLoaded();

        // Step 4: Create and show the main window with injected ViewModel
        var mainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    /// <summary>
    /// Cleanup when application exits.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        base.OnExit(e);
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
        services.AddTransient<ThemingViewModel>();
    }
}