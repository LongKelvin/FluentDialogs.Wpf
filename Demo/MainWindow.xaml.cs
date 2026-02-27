using System.Windows;

using FluentDialogs.Demo.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace FluentDialogs.Demo;

/// <summary>
/// Main window for the FluentDialogs demo application.
/// </summary>
/// <remarks>
/// This window demonstrates all features of FluentDialogs.Wpf library.
/// The ViewModel is injected via dependency injection in App.xaml.cs.
/// </remarks>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // Wire up the ThemingPage DataContext via DI
        ThemingContent.DataContext = App.Services.GetRequiredService<ThemingViewModel>();
    }

    /// <summary>
    /// Navigate to the Dialogs page.
    /// </summary>
    private void NavDialogs_Click(object sender, RoutedEventArgs e)
    {
        DialogsContent.Visibility = Visibility.Visible;
        ThemingContent.Visibility = Visibility.Collapsed;

        NavDialogsBtn.Tag = null;
        NavThemingBtn.Tag = "Inactive";
    }

    /// <summary>
    /// Navigate to the Theming page.
    /// </summary>
    private void NavTheming_Click(object sender, RoutedEventArgs e)
    {
        DialogsContent.Visibility = Visibility.Collapsed;
        ThemingContent.Visibility = Visibility.Visible;

        NavDialogsBtn.Tag = "Inactive";
        NavThemingBtn.Tag = null;
    }
}
