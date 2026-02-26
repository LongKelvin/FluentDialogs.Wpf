using System.Windows;
using System.Windows.Threading;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;
using FluentDialogs.ViewModels;
using FluentDialogs.Views;
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;
using DialogResult = FluentDialogs.Models.DialogResult;

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
    private readonly IFluentDialogThemeService? _v2ThemeService;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxService"/> class with v2 theme support.
    /// </summary>
    /// <param name="themeService">The v1 theme service (legacy adapter).</param>
    /// <param name="v2ThemeService">The v2 theme service for advanced theming.</param>
    public MessageBoxService(IMessageBoxThemeService themeService, IFluentDialogThemeService v2ThemeService)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _v2ThemeService = v2ThemeService ?? throw new ArgumentNullException(nameof(v2ThemeService));
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> ShowAsync(MessageBoxOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Validate();

        return ShowDialogOnUiThreadAsync(options);
    }

    /// <inheritdoc/>
    public async Task<DialogResult> ShowExtendedAsync(MessageBoxOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Validate();

        return await ShowExtendedDialogOnUiThreadAsync(options);
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> InfoAsync(string message, string? title = null)
    {
        var options = new MessageBoxOptions
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
        var options = new MessageBoxOptions
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
        var options = new MessageBoxOptions
        {
            Title = "Error",
            Message = message,
            Icon = MessageBoxIcon.Error,
            Buttons = MessageBoxButtons.OK,
            Exception = exception
        };

        return ShowAsync(options);
    }

    /// <inheritdoc/>
    public Task<MessageBoxResult> WarningAsync(string message, string? title = null)
    {
        var options = new MessageBoxOptions
        {
            Title = title ?? "Warning",
            Message = message,
            Icon = MessageBoxIcon.Warning,
            Buttons = MessageBoxButtons.OK
        };

        return ShowAsync(options);
    }

    /// <inheritdoc/>
    public Task<DialogResult> ConfirmWithCheckboxAsync(string message, string checkboxText, string? title = null)
    {
        var options = new MessageBoxOptions
        {
            Title = title ?? "Confirm",
            Message = message,
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.YesNo,
            CheckboxText = checkboxText
        };

        return ShowExtendedAsync(options);
    }

    /// <inheritdoc/>
    public Task<DialogResult> InputAsync(string message, string placeholder, string? defaultValue = null, string? title = null, bool isPassword = false)
    {
        var options = new MessageBoxOptions
        {
            Title = title ?? "Input",
            Message = message,
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.OKCancel,
            InputPlaceholder = placeholder,
            InputDefaultValue = defaultValue,
            InputIsPassword = isPassword
        };

        return ShowExtendedAsync(options);
    }

    /// <inheritdoc/>
    public Task<DialogResult> SelectAsync<T>(string message, IEnumerable<T> items, string? displayMemberPath = null, int defaultIndex = 0, string? title = null)
    {
        var options = new MessageBoxOptions
        {
            Title = title ?? "Select",
            Message = message,
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.OKCancel,
            SelectionItems = items.Cast<object>().ToList().AsReadOnly(),
            SelectionDisplayMemberPath = displayMemberPath,
            SelectionDefaultIndex = defaultIndex
        };

        return ShowExtendedAsync(options);
    }

    /// <inheritdoc/>
    public Task<DialogResult> LicenseAsync(string title, string message, string detailedText, bool requireScrollToBottom = true)
    {
        var options = new MessageBoxOptions
        {
            Title = title,
            Message = message,
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OKCancel,
            DetailedText = detailedText,
            RequireScrollToBottom = requireScrollToBottom
        };

        return ShowExtendedAsync(options);
    }

    /// <inheritdoc/>
    public Task<DialogResult> TimeoutAsync(string message, int timeoutSeconds, MessageBoxResult timeoutResult = MessageBoxResult.Cancel, string? title = null)
    {
        var options = new MessageBoxOptions
        {
            Title = title ?? "Information",
            Message = message,
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK,
            TimeoutSeconds = timeoutSeconds,
            TimeoutResult = timeoutResult
        };

        return ShowExtendedAsync(options);
    }

    private Task<MessageBoxResult> ShowDialogOnUiThreadAsync(MessageBoxOptions options)
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
            return Task.FromResult(ShowDialogInternal(options).Result);
        }

        var taskCompletionSource = new TaskCompletionSource<MessageBoxResult>();

        dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            () =>
            {
                try
                {
                    var result = ShowDialogInternal(options);
                    taskCompletionSource.SetResult(result.Result);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

        return taskCompletionSource.Task;
    }

    private Task<DialogResult> ShowExtendedDialogOnUiThreadAsync(MessageBoxOptions options)
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

        var taskCompletionSource = new TaskCompletionSource<DialogResult>();

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

    private DialogResult ShowDialogInternal(MessageBoxOptions options)
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

        return new DialogResult
        {
            Result = window.Result,
            IsChecked = viewModel.IsCheckboxChecked,
            InputText = viewModel.InputText,
            SelectedItem = viewModel.SelectedItem,
            SelectedIndex = viewModel.SelectedIndex,
            TimedOut = viewModel.TimedOut
        };
    }

    private void ApplyThemeToWindow(MessageBoxWindow window)
    {
        // v2 path: ensure theme is loaded at App level. Windows inherit via DynamicResource.
        if (_v2ThemeService is not null)
        {
            _v2ThemeService.EnsureThemeLoaded();
            return;
        }

        // v1 fallback: add theme dictionary directly to the window's resources
        if (_themeService is not null)
        {
            var themeResources = MessageBoxThemeService.GetThemeResources(_themeService.CurrentTheme);
            window.Resources.MergedDictionaries.Add(themeResources);
        }
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

    /// <inheritdoc/>
    public Task<IProgressController> ShowProgressAsync(ProgressOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return ShowProgressOnUiThreadAsync(options);
    }

    /// <inheritdoc/>
    public async Task<T?> RunWithProgressAsync<T>(Func<IProgress<double>, CancellationToken, Task<T>> operation, ProgressOptions options)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(options);

        var controller = await ShowProgressAsync(options);
        var progress = new Progress<double>(value => controller.SetProgress(value));

        try
        {
            return await operation(progress, controller.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            return default;
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <inheritdoc/>
    public async Task RunWithProgressAsync(Func<IProgress<double>, CancellationToken, Task> operation, ProgressOptions options)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(options);

        var controller = await ShowProgressAsync(options);
        var progress = new Progress<double>(value => controller.SetProgress(value));

        try
        {
            await operation(progress, controller.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled, which is expected
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    private Task<IProgressController> ShowProgressOnUiThreadAsync(ProgressOptions options)
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
            return Task.FromResult(ShowProgressInternal(options));
        }

        var taskCompletionSource = new TaskCompletionSource<IProgressController>();

        dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            () =>
            {
                try
                {
                    var controller = ShowProgressInternal(options);
                    taskCompletionSource.SetResult(controller);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

        return taskCompletionSource.Task;
    }

    private IProgressController ShowProgressInternal(ProgressOptions options)
    {
        var window = new ProgressWindow();
        window.Initialize(options);

        ApplyThemeToProgressWindow(window);
        SetWindowOwner(window, options.Owner);

        // Show non-modal so we can return control
        window.Show();

        return window;
    }

    private void ApplyThemeToProgressWindow(ProgressWindow window)
    {
        // v2 path: ensure theme is loaded at App level
        if (_v2ThemeService is not null)
        {
            _v2ThemeService.EnsureThemeLoaded();
            return;
        }

        // v1 fallback
        if (_themeService is not null)
        {
            var themeResources = MessageBoxThemeService.GetThemeResources(_themeService.CurrentTheme);
            window.Resources.MergedDictionaries.Add(themeResources);
        }
    }
}
