using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FluentDialogs.Enums;

namespace FluentDialogs.Models;

/// <summary>
/// Configuration options for the FluentDialogs theming system.
/// Passed to <c>AddFluentDialogs(Action&lt;FluentDialogOptions&gt;)</c>.
/// </summary>
public sealed class FluentDialogOptions
{
    /// <summary>
    /// The default preset to apply at startup. Defaults to <see cref="MessageBoxTheme.Light"/>.
    /// </summary>
    public MessageBoxTheme DefaultPreset { get; set; } = MessageBoxTheme.Light;

    /// <summary>
    /// Optional custom accent/brand color. When set, overrides the Interactive semantic tokens
    /// (FDSemInteractiveDefault, Hover, Pressed) and derived tokens like Link colors.
    /// </summary>
    public Color? AccentColor { get; set; }

    /// <summary>
    /// Optional URI to a custom preset ResourceDictionary. When provided, this is loaded
    /// instead of the built-in Light/Dark presets.
    /// <para>
    /// The dictionary should override <c>FDSem*</c> keys. See Presets/Light.xaml for reference.
    /// </para>
    /// </summary>
    public Uri? CustomPresetUri { get; set; }

    /// <summary>
    /// Individual token overrides applied AFTER the preset loads.
    /// Key = token resource key (use <see cref="ThemeTokenKeys"/> constants),
    /// Value = <see cref="Color"/> value.
    /// <para>
    /// Example: <c>options.TokenOverrides[ThemeTokenKeys.InteractiveDefault] = Colors.Purple;</c>
    /// </para>
    /// </summary>
    public Dictionary<string, Color> TokenOverrides { get; set; } = new();

    /// <summary>
    /// When true, includes the Legacy/v1-compat.xaml dictionary for backward compatibility
    /// with v1 resource key names. Defaults to true.
    /// Set to false for a clean v2-only setup (smaller resource tree).
    /// </summary>
    public bool IncludeLegacyCompatibility { get; set; } = true;
}
