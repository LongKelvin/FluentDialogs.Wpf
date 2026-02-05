using FluentDialogs.Enums;

namespace FluentDialogs.Models;

/// <summary>
/// Specifies options for displaying a toast notification.
/// </summary>
/// <example>
/// <code>
/// var options = new ToastOptions
/// {
///     Message = "File saved successfully!",
///     Type = ToastType.Success,
///     Duration = TimeSpan.FromSeconds(3)
/// };
/// </code>
/// </example>
public class ToastOptions
{
    /// <summary>
    /// Gets the message text displayed in the toast.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional title displayed above the message.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the visual style of the toast.
    /// </summary>
    public ToastType Type { get; init; } = ToastType.Info;

    /// <summary>
    /// Gets the duration the toast is displayed before auto-closing.
    /// Set to <see cref="TimeSpan.Zero"/> for a toast that doesn't auto-close.
    /// </summary>
    public TimeSpan Duration { get; init; } = TimeSpan.FromSeconds(4);

    /// <summary>
    /// Gets a value indicating whether the toast can be closed by clicking.
    /// </summary>
    public bool IsClickToClose { get; init; } = true;

    /// <summary>
    /// Gets the position where the toast appears on screen.
    /// </summary>
    public ToastPosition Position { get; init; } = ToastPosition.TopRight;

    /// <summary>
    /// Gets the action to execute when the toast is clicked.
    /// The toast will close after the action is executed.
    /// </summary>
    public Action? OnClick { get; init; }

    /// <summary>
    /// Gets the action to execute when the toast is closed.
    /// </summary>
    public Action? OnClose { get; init; }
}

/// <summary>
/// Alias for <see cref="ToastOptions"/> to maintain naming consistency.
/// </summary>
public class FluentToastOptions : ToastOptions;
