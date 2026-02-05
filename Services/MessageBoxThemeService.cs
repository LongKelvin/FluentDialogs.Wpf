using System.Windows;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.Services;

/// <summary>
/// Provides runtime theme management for message box dialogs.
/// </summary>
/// <remarks>
/// This service manages the application's resource dictionaries to switch between
/// light and dark themes. Theme changes take effect immediately for all subsequently
/// displayed dialogs.
/// </remarks>
public sealed class MessageBoxThemeService : IMessageBoxThemeService
{
    private const string LightThemeUri = "/FluentDialogs.Wpf;component/Themes/FluentLight.xaml";
    private const string DarkThemeUri = "/FluentDialogs.Wpf;component/Themes/FluentDark.xaml";

    private MessageBoxTheme _currentTheme = MessageBoxTheme.Light;
    private ResourceDictionary? _currentThemeDictionary;
    private bool _isInitialized;

    /// <inheritdoc/>
    public MessageBoxTheme CurrentTheme => _currentTheme;

    /// <inheritdoc/>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <inheritdoc/>
    public void SetTheme(MessageBoxTheme theme)
    {
        if (_isInitialized && _currentTheme == theme)
        {
            return;
        }

        var themeUri = theme switch
        {
            MessageBoxTheme.Dark => DarkThemeUri,
            _ => LightThemeUri
        };

        var newThemeDictionary = new ResourceDictionary
        {
            Source = new Uri(themeUri, UriKind.Relative)
        };

        var appResources = Application.Current?.Resources;
        if (appResources == null)
        {
            return;
        }

        if (_currentThemeDictionary != null)
        {
            appResources.MergedDictionaries.Remove(_currentThemeDictionary);
        }

        appResources.MergedDictionaries.Add(newThemeDictionary);
        _currentThemeDictionary = newThemeDictionary;

        var oldTheme = _currentTheme;
        _currentTheme = theme;
        _isInitialized = true;

        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(oldTheme, theme));
    }

    /// <summary>
    /// Initializes the theme service with the default light theme.
    /// </summary>
    /// <remarks>
    /// Call this method during application startup to ensure the theme resources are loaded.
    /// </remarks>
    public void Initialize()
    {
        if (!_isInitialized)
        {
            SetTheme(MessageBoxTheme.Light);
        }
    }

    /// <summary>
    /// Gets the resource dictionary for the specified theme without applying it.
    /// </summary>
    /// <param name="theme">The theme to get resources for.</param>
    /// <returns>A new <see cref="ResourceDictionary"/> containing the theme resources.</returns>
    internal static ResourceDictionary GetThemeResources(MessageBoxTheme theme)
    {
        var themeUri = theme switch
        {
            MessageBoxTheme.Dark => DarkThemeUri,
            _ => LightThemeUri
        };

        return new ResourceDictionary
        {
            Source = new Uri(themeUri, UriKind.Relative)
        };
    }
}
