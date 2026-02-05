using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.Views;

/// <summary>
/// Individual toast notification control.
/// </summary>
public partial class ToastControl : UserControl
{
    private readonly ToastOptions _options;
    private readonly DispatcherTimer? _timer;
    private Action? _onClose;

    /// <summary>
    /// Occurs when the toast is closing.
    /// </summary>
    public event EventHandler? Closing;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastControl"/> class.
    /// </summary>
    public ToastControl(ToastOptions options)
    {
        _options = options;
        InitializeComponent();

        Initialize();

        if (options.Duration > TimeSpan.Zero)
        {
            _timer = new DispatcherTimer { Interval = options.Duration };
            _timer.Tick += (_, _) =>
            {
                _timer.Stop();
                Close();
            };
            _timer.Start();
        }
    }

    private void Initialize()
    {
        MessageText.Text = _options.Message;

        if (!string.IsNullOrEmpty(_options.Title))
        {
            TitleText.Text = _options.Title;
            TitleText.Visibility = Visibility.Visible;
        }

        _onClose = _options.OnClose;

        ApplyTypeStyle(_options.Type);

        if (_options.IsClickToClose || _options.OnClick != null)
        {
            MouseLeftButtonUp += ToastControl_MouseLeftButtonUp;
        }

        // Fade in animation
        Opacity = 0;
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        BeginAnimation(OpacityProperty, fadeIn);
    }

    private void ApplyTypeStyle(ToastType type)
    {
        var (background, icon) = type switch
        {
            ToastType.Success => (Color.FromRgb(34, 139, 34), "✓"),
            ToastType.Warning => (Color.FromRgb(255, 165, 0), "⚠"),
            ToastType.Error => (Color.FromRgb(220, 53, 69), "✕"),
            _ => (Color.FromRgb(0, 120, 212), "ℹ")
        };

        IconBorder.Background = new SolidColorBrush(background);
        IconText.Text = icon;
    }

    private void ToastControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _options.OnClick?.Invoke();
        if (_options.IsClickToClose)
        {
            Close();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        Close();
    }

    /// <summary>
    /// Closes the toast with a fade-out animation.
    /// </summary>
    public void Close()
    {
        _timer?.Stop();

        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
        fadeOut.Completed += (_, _) =>
        {
            _onClose?.Invoke();
            Closing?.Invoke(this, EventArgs.Empty);
        };
        BeginAnimation(OpacityProperty, fadeOut);
    }
}
