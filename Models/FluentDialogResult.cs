namespace FluentDialogs.Models;

/// <summary>
/// Alias for <see cref="DialogResult"/> to avoid namespace conflicts with System.Windows.Forms.DialogResult.
/// </summary>
/// <remarks>
/// Use this type when you need to avoid conflicts with System.Windows.Forms.DialogResult
/// or prefer explicit naming. This class is functionally identical to <see cref="DialogResult"/>.
/// </remarks>
/// <example>
/// <code>
/// // No need for 'using DialogResult = FluentDialogs.Models.DialogResult;'
/// FluentDialogResult result = await messageBoxService.InputAsync("Enter name:", "Name");
/// if (result.Result == MessageBoxResult.OK)
/// {
///     string name = result.InputText;
/// }
/// </code>
/// </example>
public class FluentDialogResult : DialogResult;
