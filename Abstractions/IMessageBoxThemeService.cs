using FluentDialogs.Enums;

namespace FluentDialogs.Abstractions;

/// <summary>
/// Provides methods for managing the visual theme of message box dialogs.
/// </summary>
/// <remarks>
/// This service allows runtime switching between light and dark themes.
/// The theme change affects all dialogs displayed after the change is made.
/// </remarks>
/// <example>
/// <code>
/// public class SettingsViewModel
/// {
///     private readonly IMessageBoxThemeService _themeService;
///     
///     public SettingsViewModel(IMessageBoxThemeService themeService)
///     {
///         _themeService = themeService;
///     }
///     
///     public void SetDarkMode(bool isDarkMode)
///     {
///         _themeService.SetTheme(isDarkMode ? MessageBoxTheme.Dark : MessageBoxTheme.Light);
///     }
/// }
/// </code>
/// </example>
public interface IMessageBoxThemeService
{
    /// <summary>
    /// Gets the currently active theme.
    /// </summary>
    MessageBoxTheme CurrentTheme { get; }

    /// <summary>
    /// Sets the theme for all message box dialogs.
    /// </summary>
    /// <param name="theme">The theme to apply.</param>
    void SetTheme(MessageBoxTheme theme);
}
