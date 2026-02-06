using System.Windows;
using System.Windows.Threading;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;
using FluentDialogs.Views;

namespace FluentDialogs.Services;

/// <summary>
/// Provides methods for displaying non-modal toast notifications.
/// </summary>
/// <remarks>
/// Toast notifications are displayed in a container window that floats above the application.
/// Multiple toasts can be displayed simultaneously, and they auto-close after their duration expires.
/// </remarks>
public sealed class ToastService : IToastService
{
    private ToastContainerWindow? _containerWindow;
    private readonly object _lock = new();

    /// <inheritdoc/>
    public ToastPosition DefaultPosition { get; set; } = ToastPosition.TopRight;

    /// <inheritdoc/>
    public int MaxToasts { get; set; } = 5;

    /// <inheritdoc/>
    public void Show(ToastOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var dispatcher = Application.Current?.Dispatcher;

        if (dispatcher == null)
        {
            throw new InvalidOperationException(
                "Cannot show toast: Application.Current is null. " +
                "Ensure the WPF application is running before displaying toasts.");
        }

        if (dispatcher.CheckAccess())
        {
            ShowInternal(options);
        }
        else
        {
            dispatcher.BeginInvoke(DispatcherPriority.Normal, () => ShowInternal(options));
        }
    }

    /// <inheritdoc/>
    public void ShowInfo(string message, string? title = null, TimeSpan? duration = null)
    {
        Show(new ToastOptions
        {
            Message = message,
            Title = title,
            Type = ToastType.Info,
            Duration = duration ?? TimeSpan.FromSeconds(4),
            Position = DefaultPosition
        });
    }

    /// <inheritdoc/>
    public void ShowSuccess(string message, string? title = null, TimeSpan? duration = null)
    {
        Show(new ToastOptions
        {
            Message = message,
            Title = title,
            Type = ToastType.Success,
            Duration = duration ?? TimeSpan.FromSeconds(4),
            Position = DefaultPosition
        });
    }

    /// <inheritdoc/>
    public void ShowWarning(string message, string? title = null, TimeSpan? duration = null)
    {
        Show(new ToastOptions
        {
            Message = message,
            Title = title,
            Type = ToastType.Warning,
            Duration = duration ?? TimeSpan.FromSeconds(4),
            Position = DefaultPosition
        });
    }

    /// <inheritdoc/>
    public void ShowError(string message, string? title = null, TimeSpan? duration = null)
    {
        Show(new ToastOptions
        {
            Message = message,
            Title = title,
            Type = ToastType.Error,
            Duration = duration ?? TimeSpan.FromSeconds(5),
            Position = DefaultPosition
        });
    }

    /// <inheritdoc/>
    public void CloseAll()
    {
        var dispatcher = Application.Current?.Dispatcher;

        if (dispatcher?.CheckAccess() == true)
        {
            _containerWindow?.CloseAllToasts();
        }
        else
        {
            dispatcher?.BeginInvoke(DispatcherPriority.Normal, () => _containerWindow?.CloseAllToasts());
        }
    }

    private void ShowInternal(ToastOptions options)
    {
        lock (_lock)
        {
            EnsureContainerWindow();
            _containerWindow!.AddToast(options, MaxToasts);
        }
    }

    private void EnsureContainerWindow()
    {
        if (_containerWindow == null || !_containerWindow.IsLoaded)
        {
            _containerWindow = new ToastContainerWindow
            {
                Position = DefaultPosition
            };
        }
    }
}
