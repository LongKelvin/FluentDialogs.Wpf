namespace FluentDialogs.Models;

/// <summary>
/// Provides control over an active progress dialog, allowing progress updates
/// and responding to user cancellation requests.
/// </summary>
/// <remarks>
/// This interface is returned when showing a progress dialog and allows the caller
/// to update the progress value, change the message, and check for cancellation.
/// </remarks>
/// <example>
/// <code>
/// var controller = await _messageBoxService.ShowProgressAsync(new ProgressOptions
/// {
///     Title = "Processing",
///     Message = "Starting...",
///     IsCancellable = true
/// });
/// 
/// for (int i = 0; i &lt;= 100; i++)
/// {
///     if (controller.IsCancellationRequested)
///     {
///         break;
///     }
///     
///     controller.SetProgress(i);
///     controller.SetMessage($"Processing item {i} of 100...");
///     await Task.Delay(50);
/// }
/// 
/// await controller.CloseAsync();
/// </code>
/// </example>
public interface IProgressController
{
    /// <summary>
    /// Gets a value indicating whether the user has requested cancellation.
    /// </summary>
    bool IsCancellationRequested { get; }

    /// <summary>
    /// Gets the cancellation token associated with this progress operation.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Sets the current progress value (0-100).
    /// </summary>
    /// <param name="value">The progress value between 0 and 100.</param>
    void SetProgress(double value);

    /// <summary>
    /// Sets the message displayed in the progress dialog.
    /// </summary>
    /// <param name="message">The message to display.</param>
    void SetMessage(string message);

    /// <summary>
    /// Sets whether the progress is indeterminate.
    /// </summary>
    /// <param name="isIndeterminate">True for indeterminate progress, false for determinate.</param>
    void SetIndeterminate(bool isIndeterminate);

    /// <summary>
    /// Closes the progress dialog.
    /// </summary>
    /// <returns>A task that completes when the dialog is closed.</returns>
    Task CloseAsync();
}
