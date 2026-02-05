using System.Windows;

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
    }
}
