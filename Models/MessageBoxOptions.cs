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
public class MessageBoxOptions
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

    #region Dialog Sizing and Appearance

    /// <summary>
    /// Gets the preferred width of the dialog in device-independent pixels.
    /// When not specified, the dialog auto-sizes to fit content within reasonable limits.
    /// </summary>
    public double? Width { get; init; }

    /// <summary>
    /// Gets the preferred height of the dialog in device-independent pixels.
    /// When not specified, the dialog auto-sizes to fit content within reasonable limits.
    /// </summary>
    public double? Height { get; init; }

    /// <summary>
    /// Gets the minimum width of the dialog in device-independent pixels.
    /// Defaults to 320 if not specified.
    /// </summary>
    public double? MinWidth { get; init; }

    /// <summary>
    /// Gets the minimum height of the dialog in device-independent pixels.
    /// Defaults to 150 if not specified.
    /// </summary>
    public double? MinHeight { get; init; }

    /// <summary>
    /// Gets the maximum width of the dialog in device-independent pixels.
    /// Defaults to 800 if not specified to prevent dialogs from becoming too wide.
    /// </summary>
    public double? MaxWidth { get; init; }

    /// <summary>
    /// Gets the maximum height of the dialog in device-independent pixels.
    /// Defaults to 600 if not specified to prevent dialogs from becoming too tall.
    /// </summary>
    public double? MaxHeight { get; init; }

    /// <summary>
    /// Gets the custom title bar background color.
    /// When not specified, uses the theme's default title bar color.
    /// </summary>
    public System.Windows.Media.Color? TitleBarColor { get; init; }

    #endregion

    #region Native Dialog Features

    /// <summary>
    /// Gets the checkbox text to display below the message.
    /// When set, a checkbox will be shown with this label.
    /// </summary>
    /// <example>"Don't show this message again"</example>
    public string? CheckboxText { get; init; }

    /// <summary>
    /// Gets the initial state of the checkbox.
    /// </summary>
    public bool CheckboxChecked { get; init; }

    /// <summary>
    /// Gets the hyperlink text to display below the message.
    /// When set along with <see cref="HyperlinkUrl"/>, a clickable link will be shown.
    /// </summary>
    /// <example>"Learn more about this feature"</example>
    public string? HyperlinkText { get; init; }

    /// <summary>
    /// Gets the URL to open when the hyperlink is clicked.
    /// </summary>
    public string? HyperlinkUrl { get; init; }

    /// <summary>
    /// Gets the placeholder text for the input field.
    /// When set, an input text box will be shown with this placeholder.
    /// </summary>
    public string? InputPlaceholder { get; init; }

    /// <summary>
    /// Gets the initial value for the input field.
    /// </summary>
    public string? InputDefaultValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether the input should be masked (password mode).
    /// </summary>
    public bool InputIsPassword { get; init; }

    /// <summary>
    /// Gets the list of items for selection dialogs.
    /// When set, a selection list will be shown.
    /// </summary>
    public IReadOnlyList<object>? SelectionItems { get; init; }

    /// <summary>
    /// Gets the display member path for selection items.
    /// Used when SelectionItems contains complex objects.
    /// </summary>
    public string? SelectionDisplayMemberPath { get; init; }

    /// <summary>
    /// Gets the initially selected index for selection dialogs.
    /// </summary>
    public int SelectionDefaultIndex { get; init; } = -1;

    /// <summary>
    /// Gets the timeout in seconds after which the dialog auto-closes.
    /// Set to 0 or negative to disable auto-close.
    /// </summary>
    public int TimeoutSeconds { get; init; }

    /// <summary>
    /// Gets the result to return when the dialog times out.
    /// </summary>
    public System.Windows.MessageBoxResult TimeoutResult { get; init; } = System.Windows.MessageBoxResult.Cancel;

    /// <summary>
    /// Gets the long-form content text (e.g., license agreement, disclaimer).
    /// When set, a scrollable text area will be shown.
    /// </summary>
    public string? DetailedText { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user must scroll to the bottom 
    /// of detailed text before accepting (for license agreements).
    /// </summary>
    public bool RequireScrollToBottom { get; init; }

    /// <summary>
    /// Gets the list of items for dropdown dialogs.
    /// When set, a ComboBox-style dropdown will be shown.
    /// </summary>
    public IReadOnlyList<object>? DropdownItems { get; init; }

    /// <summary>
    /// Gets the display member path for dropdown items.
    /// Used when DropdownItems contains complex objects.
    /// </summary>
    public string? DropdownDisplayMemberPath { get; init; }

    /// <summary>
    /// Gets the initially selected index for dropdown dialogs.
    /// </summary>
    public int DropdownDefaultIndex { get; init; } = -1;

    /// <summary>
    /// Gets a value indicating whether the dialog is resizable.
    /// When true, the user can resize the dialog by dragging its edges.
    /// Useful for license agreement or large content dialogs.
    /// </summary>
    public bool IsResizable { get; init; }

    #endregion

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
