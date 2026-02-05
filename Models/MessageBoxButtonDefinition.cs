using System.Windows;
using System.Windows.Input;
using FluentDialogs.Enums;

namespace FluentDialogs.Models;

/// <summary>
/// Defines a custom button for a message box dialog.
/// </summary>
/// <example>
/// <code>
/// var customButton = new MessageBoxButtonDefinition
/// {
///     Text = "Save",
///     Result = MessageBoxResult.Yes,
///     Style = ButtonStyle.Primary,
///     IsDefault = true
/// };
/// </code>
/// </example>
public sealed class MessageBoxButtonDefinition
{
    /// <summary>
    /// Gets the text displayed on the button.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the result value returned when this button is clicked.
    /// </summary>
    public MessageBoxResult Result { get; init; } = MessageBoxResult.None;

    /// <summary>
    /// Gets a value indicating whether this button is the default button.
    /// The default button is activated when the user presses Enter.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Gets a value indicating whether this button is the cancel button.
    /// The cancel button is activated when the user presses Escape.
    /// </summary>
    public bool IsCancel { get; init; }

    /// <summary>
    /// Gets the command to execute when the button is clicked.
    /// The command is executed before the dialog closes.
    /// </summary>
    public ICommand? Command { get; init; }

    /// <summary>
    /// Gets the parameter to pass to the command when executed.
    /// </summary>
    public object? CommandParameter { get; init; }

    /// <summary>
    /// Gets the visual style of the button.
    /// </summary>
    public ButtonStyle Style { get; init; } = ButtonStyle.Default;
}
