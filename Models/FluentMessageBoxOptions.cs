namespace FluentDialogs.Models;

/// <summary>
/// Alias for <see cref="MessageBoxOptions"/> to avoid namespace conflicts with System.Windows types.
/// </summary>
/// <remarks>
/// Use this type when you need to avoid conflicts with System.Windows.MessageBoxOptions
/// or prefer explicit naming. This class is functionally identical to <see cref="MessageBoxOptions"/>.
/// </remarks>
/// <example>
/// <code>
/// // No need for 'using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;'
/// var options = new FluentMessageBoxOptions
/// {
///     Title = "Confirm",
///     Message = "Are you sure?",
///     Buttons = MessageBoxButtons.YesNo
/// };
/// </code>
/// </example>
public class FluentMessageBoxOptions : MessageBoxOptions;
