using System.Windows;
using System.Windows.Threading;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.ViewModels;
using FluentDialogs.Views;

namespace FluentDialogs.Services;

/// <summary>
/// Provides methods for displaying message box dialogs with Windows 11 Fluent Design.
/// </summary>
/// <remarks>
/// This service is designed to be used with dependency injection and handles UI thread
/// marshaling automatically. All dialog methods are async-safe and can be called from
/// any thread.
/// </remarks>
public sealed class MessageBoxService : IMessageBoxService
{
    private readonly IMessageBoxThemeService? _themeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxService"/> class.
    /// </summary>
    public MessageBoxService()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxService"/> class with theme support.
    /// </summary>
    /// <param name="themeService">The theme service for applying the current theme to dialogs.</param>
    public MessageBoxService(IMessageBoxThemeService themeService)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> ShowAsync(Models.MessageBoxOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Validate();

        return ShowDialogOnUiThreadAsync(options);
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> InfoAsync(string message, string? title = null)
    {
        var options = new Models.MessageBoxOptions
        {
            Title = title ?? "Information",
            Message = message,
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK
        };

        return ShowAsync(options);
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> ConfirmAsync(string message, string? title = null)
    {
        var options = new Models.MessageBoxOptions
        {
            Title = title ?? "Confirm",
            Message = message,
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.YesNo
        };

        return ShowAsync(options);
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> ErrorAsync(string message, Exception? exception = null)
    {
        var options = new Models.MessageBoxOptions
        {
            Title = "Error",
            Message = message,
            Icon = MessageBoxIcon.Error,
            Buttons = MessageBoxButtons.OK,
            Exception = exception
        };

        return ShowAsync(options);
    }

    private Task<MessageBoxResult> ShowDialogOnUiThreadAsync(Models.MessageBoxOptions options)
    {
        var dispatcher = Application.Current?.Dispatcher;

        if (dispatcher == null)
        {
            throw new InvalidOperationException(
                "Cannot show dialog: Application.Current is null. " +
                "Ensure the WPF application is running before displaying dialogs.");
        }

        if (dispatcher.CheckAccess())
        {
            return Task.FromResult(ShowDialogInternal(options));
        }

        var taskCompletionSource = new TaskCompletionSource<MessageBoxResult>();

        dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            () =>
            {
                try
                {
                    var result = ShowDialogInternal(options);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

        return taskCompletionSource.Task;
    }

    private MessageBoxResult ShowDialogInternal(Models.MessageBoxOptions options)
    {
        var viewModel = new MessageBoxViewModel();
        viewModel.Initialize(options);

        var window = new MessageBoxWindow
        {
            DataContext = viewModel
        };

        ApplyThemeToWindow(window);
        SetWindowOwner(window, options.Owner);

        window.ShowDialog();

        return window.Result;
    }

    private void ApplyThemeToWindow(MessageBoxWindow window)
    {
        if (_themeService == null)
        {
            return;
        }

        var themeResources = MessageBoxThemeService.GetThemeResources(_themeService.CurrentTheme);
        window.Resources.MergedDictionaries.Add(themeResources);
    }

    private static void SetWindowOwner(Window window, Window? ownerWindow)
    {
        if (ownerWindow != null)
        {
            window.Owner = ownerWindow;
            return;
        }

        var activeWindow = GetActiveWindow();
        if (activeWindow != null && activeWindow != window)
        {
            window.Owner = activeWindow;
        }
    }

    private static Window? GetActiveWindow()
    {
        var application = Application.Current;
        if (application == null)
        {
            return null;
        }

        var activeWindow = application.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

        return activeWindow ?? application.MainWindow;
    }
}
