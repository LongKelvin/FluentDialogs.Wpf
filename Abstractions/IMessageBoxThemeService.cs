using FluentDialogs.Enums;
using FluentDialogs.Models;

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

    /// <summary>
    /// Occurs when the theme is changed.
    /// </summary>
    /// <remarks>
    /// Subscribe to this event to respond to theme changes across the application,
    /// such as updating UI elements or persisting the theme preference.
    /// </remarks>
    /// <example>
    /// <code>
    /// _themeService.ThemeChanged += (sender, e) =>
    /// {
    ///     Console.WriteLine($"Theme changed from {e.OldTheme} to {e.NewTheme}");
    /// };
    /// </code>
    /// </example>
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}
