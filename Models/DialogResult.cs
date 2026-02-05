using System.Windows;

namespace FluentDialogs.Models;

/// <summary>
/// Represents the result of a dialog with additional data.
/// </summary>
/// <remarks>
/// This class extends the standard MessageBoxResult to include additional
/// information such as checkbox state, input text, or selected items.
/// </remarks>
public sealed class DialogResult
{
    /// <summary>
    /// Gets the button result (OK, Cancel, Yes, No, etc.).
    /// </summary>
    public MessageBoxResult Result { get; init; } = MessageBoxResult.None;

    /// <summary>
    /// Gets the state of the checkbox if one was displayed.
    /// </summary>
    public bool IsChecked { get; init; }

    /// <summary>
    /// Gets the input text if an input field was displayed.
    /// </summary>
    public string? InputText { get; init; }

    /// <summary>
    /// Gets the selected item if a selection list was displayed.
    /// </summary>
    public object? SelectedItem { get; init; }

    /// <summary>
    /// Gets the index of the selected item if a selection list was displayed.
    /// Returns -1 if no selection was made.
    /// </summary>
    public int SelectedIndex { get; init; } = -1;

    /// <summary>
    /// Gets a value indicating whether the dialog was closed due to timeout.
    /// </summary>
    public bool TimedOut { get; init; }

    /// <summary>
    /// Creates a DialogResult from a simple MessageBoxResult.
    /// </summary>
    /// <param name="result">The message box result.</param>
    /// <returns>A new DialogResult instance.</returns>
    public static DialogResult FromResult(MessageBoxResult result) => new() { Result = result };

    /// <summary>
    /// Implicitly converts a DialogResult to a MessageBoxResult.
    /// </summary>
    /// <param name="dialogResult">The dialog result.</param>
    public static implicit operator MessageBoxResult(DialogResult dialogResult) => dialogResult.Result;
}
