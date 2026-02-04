using System.Windows;
using System.Windows.Media;
using FluentDialogs.Enums;

namespace FluentDialogs.Models;

/// <summary>
/// Specifies options for displaying a message box dialog.
/// </summary>
/// <example>
/// <code>
/// var options = new MessageBoxOptions
/// {
///     Title = "Confirm Action",
///     Message = "Are you sure you want to proceed?",
///     Icon = MessageBoxIcon.Question,
///     Buttons = MessageBoxButtons.YesNo
/// };
/// </code>
/// </example>
public sealed class MessageBoxOptions
{
    /// <summary>
    /// Gets the title text displayed in the dialog title bar.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the main message text displayed in the dialog.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the icon displayed in the dialog.
    /// </summary>
    public MessageBoxIcon Icon { get; init; } = MessageBoxIcon.None;

    /// <summary>
    /// Gets the standard button configuration for the dialog.
    /// Cannot be used together with <see cref="CustomButtons"/>.
    /// </summary>
    public MessageBoxButtons? Buttons { get; init; }

    /// <summary>
    /// Gets the custom button definitions for the dialog.
    /// Cannot be used together with <see cref="Buttons"/>.
    /// </summary>
    public IReadOnlyList<MessageBoxButtonDefinition>? CustomButtons { get; init; }

    /// <summary>
    /// Gets optional custom content to display below the message.
    /// Can be any WPF content such as a checkbox, hyperlink, or custom control.
    /// </summary>
    public object? Content { get; init; }

    /// <summary>
    /// Gets the owner window for the dialog.
    /// The dialog will be centered on and modal to this window.
    /// </summary>
    public Window? Owner { get; init; }

    /// <summary>
    /// Gets optional custom icon geometry to display instead of the standard icon.
    /// When set, this overrides the <see cref="Icon"/> property.
    /// </summary>
    public Geometry? IconContent { get; init; }

    /// <summary>
    /// Gets the exception to display in the dialog.
    /// Used primarily with error dialogs to show exception details.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Validates the options and throws an exception if the configuration is invalid.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when both <see cref="Buttons"/> and <see cref="CustomButtons"/> are set,
    /// when no buttons are specified, when multiple default buttons exist,
    /// or when multiple cancel buttons exist.
    /// </exception>
    internal void Validate()
    {
        if (Buttons.HasValue && CustomButtons is { Count: > 0 })
        {
            throw new ArgumentException("CustomButtons and Buttons cannot both be set.");
        }

        if (!Buttons.HasValue && (CustomButtons is null || CustomButtons.Count == 0))
        {
            throw new ArgumentException("At least one button must be specified. Set either Buttons or CustomButtons.");
        }

        if (CustomButtons is { Count: > 0 })
        {
            ValidateCustomButtons(CustomButtons);
        }
    }

    private static void ValidateCustomButtons(IReadOnlyList<MessageBoxButtonDefinition> buttons)
    {
        var defaultCount = 0;
        var cancelCount = 0;

        foreach (var button in buttons)
        {
            if (button.IsDefault)
            {
                defaultCount++;
            }

            if (button.IsCancel)
            {
                cancelCount++;
            }
        }

        if (defaultCount > 1)
        {
            throw new ArgumentException("Only one button can be marked as IsDefault.");
        }

        if (cancelCount > 1)
        {
            throw new ArgumentException("Only one button can be marked as IsCancel.");
        }
    }
}
