using System;
using System.Windows;
using System.Windows.Media;
using FluentDialogs.Enums;

namespace FluentDialogs.Abstractions;

/// <summary>
/// v2 theme service interface for the FluentDialogs theming system.
/// Provides preset switching, individual token overrides, and accent color management.
/// </summary>
public interface IFluentDialogThemeService
{
    /// <summary>
    /// Gets the currently active preset (Light or Dark).
    /// </summary>
    MessageBoxTheme CurrentPreset { get; }

    /// <summary>
    /// Raised after the active preset changes.
    /// </summary>
    event EventHandler<ThemePresetChangedEventArgs>? PresetChanged;

    /// <summary>
    /// Switches to a built-in preset (Light or Dark).
    /// Must be called from the UI thread.
    /// </summary>
    /// <param name="preset">The preset to apply.</param>
    void ApplyPreset(MessageBoxTheme preset);

    /// <summary>
    /// Loads a custom preset from a ResourceDictionary URI.
    /// The dictionary should override FDSem* keys.
    /// </summary>
    /// <param name="presetUri">Pack URI to the preset ResourceDictionary.</param>
    /// <param name="presetName">Optional display name for the preset.</param>
    void ApplyCustomPreset(Uri presetUri, string? presetName = null);

    /// <summary>
    /// Sets a single semantic token override at runtime.
    /// Overrides persist across preset switches until cleared.
    /// </summary>
    /// <param name="tokenKey">Semantic color token key (use <see cref="ThemeTokenKeys"/> constants).</param>
    /// <param name="color">The color value to set.</param>
    void SetToken(string tokenKey, Color color);

    /// <summary>
    /// Sets the accent/brand color, updating Interactive and Link semantic tokens.
    /// Derives hover/pressed states automatically.
    /// </summary>
    /// <param name="accentColor">The accent color.</param>
    void SetAccentColor(Color accentColor);

    /// <summary>
    /// Clears all runtime token overrides, reverting to the active preset's values.
    /// </summary>
    void ClearOverrides();

    /// <summary>
    /// Ensures the theme ResourceDictionary is loaded into the specified dictionary
    /// (typically Application.Current.Resources). Called once at startup.
    /// </summary>
    /// <param name="target">The target ResourceDictionary to load into. If null, uses Application.Current.Resources.</param>
    void EnsureThemeLoaded(ResourceDictionary? target = null);
}

/// <summary>
/// Event arguments for the <see cref="IFluentDialogThemeService.PresetChanged"/> event.
/// </summary>
public sealed class ThemePresetChangedEventArgs : EventArgs
{
    /// <summary>
    /// The preset that was active before the change.
    /// </summary>
    public MessageBoxTheme OldPreset { get; }

    /// <summary>
    /// The new active preset.
    /// </summary>
    public MessageBoxTheme NewPreset { get; }

    /// <summary>
    /// If a custom preset was applied, this is its display name; otherwise null.
    /// </summary>
    public string? CustomPresetName { get; }

    public ThemePresetChangedEventArgs(MessageBoxTheme oldPreset, MessageBoxTheme newPreset, string? customPresetName = null)
    {
        OldPreset = oldPreset;
        NewPreset = newPreset;
        CustomPresetName = customPresetName;
    }
}
