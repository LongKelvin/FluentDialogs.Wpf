using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.Abstractions;

/// <summary>
/// Provides methods for displaying non-modal toast notifications.
/// </summary>
/// <remarks>
/// Toast notifications are lightweight, non-blocking messages that appear
/// temporarily on screen. They are ideal for status updates, confirmations,
/// and non-critical information that doesn't require user interaction.
/// </remarks>
/// <example>
/// <code>
/// public class MyViewModel
/// {
///     private readonly IToastService _toastService;
///     
///     public MyViewModel(IToastService toastService)
///     {
///         _toastService = toastService;
///     }
///     
///     public async Task SaveAsync()
///     {
///         await PerformSave();
///         _toastService.ShowSuccess("File saved successfully!");
///     }
/// }
/// </code>
/// </example>
public interface IToastService
{
    /// <summary>
    /// Shows a toast notification with the specified options.
    /// </summary>
    /// <param name="options">The options that configure the toast appearance and behavior.</param>
    void Show(ToastOptions options);

    /// <summary>
    /// Shows an informational toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The optional title.</param>
    /// <param name="duration">The optional duration. Defaults to 4 seconds.</param>
    void ShowInfo(string message, string? title = null, TimeSpan? duration = null);

    /// <summary>
    /// Shows a success toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The optional title.</param>
    /// <param name="duration">The optional duration. Defaults to 4 seconds.</param>
    void ShowSuccess(string message, string? title = null, TimeSpan? duration = null);

    /// <summary>
    /// Shows a warning toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The optional title.</param>
    /// <param name="duration">The optional duration. Defaults to 4 seconds.</param>
    void ShowWarning(string message, string? title = null, TimeSpan? duration = null);

    /// <summary>
    /// Shows an error toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The optional title.</param>
    /// <param name="duration">The optional duration. Defaults to 5 seconds.</param>
    void ShowError(string message, string? title = null, TimeSpan? duration = null);

    /// <summary>
    /// Closes all currently displayed toast notifications.
    /// </summary>
    void CloseAll();

    /// <summary>
    /// Gets or sets the default position for toast notifications.
    /// </summary>
    ToastPosition DefaultPosition { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of toasts displayed simultaneously.
    /// Older toasts are removed when the limit is exceeded.
    /// </summary>
    int MaxToasts { get; set; }
}
