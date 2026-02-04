using System.Windows;
using FluentDialogs.Models;

namespace FluentDialogs.Abstractions;

/// <summary>
/// Provides methods for displaying message box dialogs.
/// </summary>
/// <remarks>
/// This service is designed to be used with dependency injection and provides
/// async-safe methods for displaying dialogs from ViewModels.
/// </remarks>
/// <example>
/// <code>
/// public class MyViewModel
/// {
///     private readonly IMessageBoxService _messageBoxService;
///     
///     public MyViewModel(IMessageBoxService messageBoxService)
///     {
///         _messageBoxService = messageBoxService;
///     }
///     
///     public async Task SaveAsync()
///     {
///         var result = await _messageBoxService.ConfirmAsync(
///             "Save changes?",
///             "Confirm Save"
///         );
///         
///         if (result == MessageBoxResult.Yes)
///         {
///             // Perform save operation
///         }
///     }
/// }
/// </code>
/// </example>
public interface IMessageBoxService
{
    /// <summary>
    /// Displays a message box dialog with the specified options.
    /// </summary>
    /// <param name="options">The options that configure the message box appearance and behavior.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the options configuration is invalid.</exception>
    Task<MessageBoxResult> ShowAsync(Models.MessageBoxOptions options);

    /// <summary>
    /// Displays an informational message box with an OK button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The optional title for the dialog. Defaults to "Information" if not specified.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's response.</returns>
    Task<MessageBoxResult> InfoAsync(string message, string? title = null);

    /// <summary>
    /// Displays a confirmation message box with Yes and No buttons.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The optional title for the dialog. Defaults to "Confirm" if not specified.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's response.</returns>
    Task<MessageBoxResult> ConfirmAsync(string message, string? title = null);

    /// <summary>
    /// Displays an error message box with optional exception details.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    /// <param name="exception">The optional exception to display. When provided, the dialog includes expandable exception details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's response.</returns>
    Task<MessageBoxResult> ErrorAsync(string message, Exception? exception = null);
}
