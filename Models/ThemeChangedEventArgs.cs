using FluentDialogs.Enums;

namespace FluentDialogs.Models;

/// <summary>
/// Provides data for the <see cref="Abstractions.IMessageBoxThemeService.ThemeChanged"/> event.
/// </summary>
public sealed class ThemeChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the previous theme before the change.
    /// </summary>
    public MessageBoxTheme OldTheme { get; }

    /// <summary>
    /// Gets the new theme after the change.
    /// </summary>
    public MessageBoxTheme NewTheme { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldTheme">The previous theme.</param>
    /// <param name="newTheme">The new theme.</param>
    public ThemeChangedEventArgs(MessageBoxTheme oldTheme, MessageBoxTheme newTheme)
    {
        OldTheme = oldTheme;
        NewTheme = newTheme;
    }
}
