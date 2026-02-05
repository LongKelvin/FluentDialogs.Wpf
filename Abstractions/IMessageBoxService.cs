using System.Windows;
using FluentDialogs.Models;
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;
using DialogResult = FluentDialogs.Models.DialogResult;

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
    Task<MessageBoxResult> ShowAsync(MessageBoxOptions options);

    /// <summary>
    /// Displays a message box dialog with the specified options and returns extended result data.
    /// </summary>
    /// <param name="options">The options that configure the message box appearance and behavior.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the extended dialog result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the options configuration is invalid.</exception>
    Task<DialogResult> ShowExtendedAsync(MessageBoxOptions options);

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

    /// <summary>
    /// Displays a warning message box with an OK button.
    /// </summary>
    /// <param name="message">The warning message to display.</param>
    /// <param name="title">The optional title for the dialog. Defaults to "Warning" if not specified.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's response.</returns>
    Task<MessageBoxResult> WarningAsync(string message, string? title = null);

    /// <summary>
    /// Displays a confirmation dialog with a checkbox (e.g., "Don't show again").
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="checkboxText">The text to display next to the checkbox.</param>
    /// <param name="title">The optional title for the dialog.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the button result and checkbox state.</returns>
    Task<DialogResult> ConfirmWithCheckboxAsync(string message, string checkboxText, string? title = null);

    /// <summary>
    /// Displays an input dialog with a text box.
    /// </summary>
    /// <param name="message">The prompt message to display.</param>
    /// <param name="placeholder">The placeholder text for the input field.</param>
    /// <param name="defaultValue">The initial value in the input field.</param>
    /// <param name="title">The optional title for the dialog.</param>
    /// <param name="isPassword">Whether to mask the input as a password.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the input text.</returns>
    Task<DialogResult> InputAsync(string message, string placeholder, string? defaultValue = null, string? title = null, bool isPassword = false);

    /// <summary>
    /// Displays a selection dialog with a list of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the selection list.</typeparam>
    /// <param name="message">The prompt message to display.</param>
    /// <param name="items">The items to display in the selection list.</param>
    /// <param name="displayMemberPath">The property path to display for complex objects.</param>
    /// <param name="defaultIndex">The initially selected index.</param>
    /// <param name="title">The optional title for the dialog.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the selected item.</returns>
    Task<DialogResult> SelectAsync<T>(string message, IEnumerable<T> items, string? displayMemberPath = null, int defaultIndex = 0, string? title = null);

    /// <summary>
    /// Displays a license agreement or disclaimer dialog that requires scrolling to accept.
    /// </summary>
    /// <param name="title">The title for the dialog.</param>
    /// <param name="message">The brief message or heading.</param>
    /// <param name="detailedText">The full license or disclaimer text.</param>
    /// <param name="requireScrollToBottom">Whether the user must scroll to the bottom before accepting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains whether the user accepted.</returns>
    Task<DialogResult> LicenseAsync(string title, string message, string detailedText, bool requireScrollToBottom = true);

    /// <summary>
    /// Displays a dialog with a timeout that auto-closes.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="timeoutSeconds">The number of seconds before auto-close.</param>
    /// <param name="timeoutResult">The result to return when the dialog times out.</param>
    /// <param name="title">The optional title for the dialog.</param>
    /// <returns>A task that represents the asynchronous operation. The task result includes whether the dialog timed out.</returns>
    Task<DialogResult> TimeoutAsync(string message, int timeoutSeconds, MessageBoxResult timeoutResult = MessageBoxResult.Cancel, string? title = null);

    /// <summary>
    /// Displays a progress dialog and returns a controller for updating progress.
    /// </summary>
    /// <param name="options">The options that configure the progress dialog.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a controller for updating the progress.</returns>
    /// <remarks>
    /// The returned <see cref="IProgressController"/> allows you to update the progress value,
    /// change the message, and check for user cancellation. You must call <see cref="IProgressController.CloseAsync"/>
    /// when the operation is complete.
    /// </remarks>
    /// <example>
    /// <code>
    /// var controller = await _messageBoxService.ShowProgressAsync(new ProgressOptions
    /// {
    ///     Title = "Processing",
    ///     Message = "Please wait...",
    ///     IsCancellable = true
    /// });
    /// 
    /// try
    /// {
    ///     for (int i = 0; i &lt;= 100; i++)
    ///     {
    ///         controller.CancellationToken.ThrowIfCancellationRequested();
    ///         controller.SetProgress(i);
    ///         await Task.Delay(50);
    ///     }
    /// }
    /// finally
    /// {
    ///     await controller.CloseAsync();
    /// }
    /// </code>
    /// </example>
    Task<IProgressController> ShowProgressAsync(ProgressOptions options);

    /// <summary>
    /// Displays a progress dialog and executes the specified operation with progress reporting.
    /// </summary>
    /// <typeparam name="T">The type of the operation result.</typeparam>
    /// <param name="operation">The async operation to execute. Receives an IProgress for reporting and a CancellationToken.</param>
    /// <param name="options">The options that configure the progress dialog.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the operation result or default if cancelled.</returns>
    /// <example>
    /// <code>
    /// var result = await _messageBoxService.RunWithProgressAsync(
    ///     async (progress, cancellationToken) =>
    ///     {
    ///         for (int i = 0; i &lt;= 100; i++)
    ///         {
    ///             cancellationToken.ThrowIfCancellationRequested();
    ///             progress.Report(i);
    ///             await Task.Delay(50);
    ///         }
    ///         return "Completed!";
    ///     },
    ///     new ProgressOptions { Title = "Processing" }
    /// );
    /// </code>
    /// </example>
    Task<T?> RunWithProgressAsync<T>(Func<IProgress<double>, CancellationToken, Task<T>> operation, ProgressOptions options);

    /// <summary>
    /// Displays a progress dialog and executes the specified operation with progress reporting.
    /// </summary>
    /// <param name="operation">The async operation to execute. Receives an IProgress for reporting and a CancellationToken.</param>
    /// <param name="options">The options that configure the progress dialog.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RunWithProgressAsync(Func<IProgress<double>, CancellationToken, Task> operation, ProgressOptions options);
}
