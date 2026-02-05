namespace FluentDialogs.Models;

/// <summary>
/// Specifies options for displaying a progress dialog.
/// </summary>
/// <example>
/// <code>
/// var options = new ProgressOptions
/// {
///     Title = "Processing",
///     Message = "Please wait...",
///     IsIndeterminate = false,
///     IsCancellable = true
/// };
/// </code>
/// </example>
public class ProgressOptions
{
    /// <summary>
    /// Gets the title text displayed in the dialog title bar.
    /// </summary>
    public string Title { get; init; } = "Progress";

    /// <summary>
    /// Gets the message text displayed above the progress bar.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the progress is indeterminate (unknown duration).
    /// When true, the progress bar shows a continuous animation instead of percentage.
    /// </summary>
    public bool IsIndeterminate { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the operation can be cancelled by the user.
    /// When true, a Cancel button is displayed.
    /// </summary>
    public bool IsCancellable { get; init; } = true;

    /// <summary>
    /// Gets the owner window for the dialog.
    /// The dialog will be centered on and modal to this window.
    /// </summary>
    public System.Windows.Window? Owner { get; init; }

    /// <summary>
    /// Gets a value indicating whether to show percentage text on the progress bar.
    /// Only applicable when <see cref="IsIndeterminate"/> is false.
    /// </summary>
    public bool ShowPercentage { get; init; } = true;

    /// <summary>
    /// Gets the initial progress value (0-100).
    /// </summary>
    public double InitialProgress { get; init; }

    /// <summary>
    /// Gets the preferred width of the dialog in device-independent pixels.
    /// </summary>
    public double? Width { get; init; }
}

/// <summary>
/// Alias for <see cref="ProgressOptions"/> to maintain naming consistency.
/// </summary>
public class FluentProgressOptions : ProgressOptions;
