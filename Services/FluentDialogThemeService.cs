using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.Services;

/// <summary>
/// v2 implementation of the FluentDialogs theme service.
/// Manages preset switching, accent color, and runtime token overrides.
/// Thread-safe: all mutations verify UI-thread affinity.
/// </summary>
/// <remarks>
/// <b>Key architecture decision:</b> Presets and runtime overrides are injected into
/// <c>FluentDialogs.Theme.xaml</c>'s own <see cref="ResourceDictionary.MergedDictionaries"/>
/// (not into Application.Resources directly). This guarantees that DynamicResource
/// expressions on brushes in <c>_Brushes.xaml</c> resolve within the same dictionary
/// scope and update correctly when presets are swapped at runtime.
/// </remarks>
public sealed class FluentDialogThemeService : IFluentDialogThemeService
{
    private static readonly Uri LightPresetUri =
        new("pack://application:,,,/FluentDialogs.Wpf;component/Themes/Presets/Light.xaml", UriKind.Absolute);

    private static readonly Uri DarkPresetUri =
        new("pack://application:,,,/FluentDialogs.Wpf;component/Themes/Presets/Dark.xaml", UriKind.Absolute);

    private static readonly Uri ThemeEntryUri =
        new("pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentDialogs.Theme.xaml", UriKind.Absolute);

    /// <summary>Substring used to detect FluentDialogs.Theme.xaml already loaded from App.xaml.</summary>
    private const string ThemeEntryFileName = "FluentDialogs.Theme.xaml";

    private readonly FluentDialogOptions _options;
    private readonly ResourceDictionary _runtimeOverrides = new();

    /// <summary>
    /// The FluentDialogs.Theme.xaml dictionary instance.
    /// Presets and runtime overrides are added to THIS dictionary's MergedDictionaries
    /// so they share the same DynamicResource resolution scope as the brush definitions.
    /// </summary>
    private ResourceDictionary? _themeDict;

    private ResourceDictionary? _activePresetDict;
    private ResourceDictionary? _target;
    private bool _isLoaded;

    public MessageBoxTheme CurrentPreset { get; private set; }

    public event EventHandler<ThemePresetChangedEventArgs>? PresetChanged;

    public FluentDialogThemeService(FluentDialogOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        CurrentPreset = options.DefaultPreset;
    }

    /// <inheritdoc/>
    public void EnsureThemeLoaded(ResourceDictionary? target = null)
    {
        VerifyUIThread();

        if (_isLoaded) return;

        _target = target ?? Application.Current?.Resources
            ?? throw new InvalidOperationException(
                "Application.Current.Resources is not available. Provide a target ResourceDictionary explicitly.");

        // 1. Find FluentDialogs.Theme.xaml already loaded from App.xaml, or load it ourselves
        _themeDict = FindExistingThemeDict(_target);
        if (_themeDict is null)
        {
            _themeDict = new ResourceDictionary { Source = ThemeEntryUri };
            _target.MergedDictionaries.Add(_themeDict);
        }

        // 2. Apply the configured default preset INSIDE the theme dictionary
        //    This ensures brushes' DynamicResource expressions (Color="{DynamicResource FDSem*}")
        //    resolve within the same scope and update correctly on preset swap.
        var presetUri = _options.CustomPresetUri ?? GetBuiltInPresetUri(_options.DefaultPreset);
        _activePresetDict = new ResourceDictionary { Source = presetUri };
        _themeDict.MergedDictionaries.Add(_activePresetDict);

        // 3. Add runtime overrides dictionary (always last = highest priority)
        _themeDict.MergedDictionaries.Add(_runtimeOverrides);

        // 4. Apply accent color if configured
        if (_options.AccentColor.HasValue)
        {
            ApplyAccentColorInternal(_options.AccentColor.Value);
        }

        // 5. Apply any initial token overrides
        foreach (var kvp in _options.TokenOverrides)
        {
            _runtimeOverrides[kvp.Key] = kvp.Value;
        }

        _isLoaded = true;

        // 6. Validate in debug builds
        ThemeValidator.ValidateIfDebug(_target);
    }

    /// <inheritdoc/>
    public void ApplyPreset(MessageBoxTheme preset)
    {
        VerifyUIThread();
        EnsureLoaded();

        var oldPreset = CurrentPreset;
        var presetUri = GetBuiltInPresetUri(preset);

        SwapPreset(new ResourceDictionary { Source = presetUri });
        CurrentPreset = preset;

        PresetChanged?.Invoke(this, new ThemePresetChangedEventArgs(oldPreset, preset));
    }

    /// <inheritdoc/>
    public void ApplyCustomPreset(Uri presetUri, string? presetName = null)
    {
        VerifyUIThread();
        EnsureLoaded();

        ArgumentNullException.ThrowIfNull(presetUri);

        var oldPreset = CurrentPreset;
        SwapPreset(new ResourceDictionary { Source = presetUri });

        // Custom presets don't map to a built-in enum; keep current as-is
        PresetChanged?.Invoke(this, new ThemePresetChangedEventArgs(oldPreset, CurrentPreset, presetName));
    }

    /// <inheritdoc/>
    public void SetToken(string tokenKey, Color color)
    {
        VerifyUIThread();
        EnsureLoaded();

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenKey);
        _runtimeOverrides[tokenKey] = color;
    }

    /// <inheritdoc/>
    public void SetAccentColor(Color accentColor)
    {
        VerifyUIThread();
        EnsureLoaded();

        ApplyAccentColorInternal(accentColor);
    }

    /// <inheritdoc/>
    public void ClearOverrides()
    {
        VerifyUIThread();
        EnsureLoaded();

        _runtimeOverrides.Clear();
    }

    // ═══════════════ Private Helpers ═══════════════

    /// <summary>
    /// Swaps the active preset dictionary inside <see cref="_themeDict"/>.
    /// Runtime overrides are removed/re-added to maintain highest priority (last position).
    /// </summary>
    private void SwapPreset(ResourceDictionary newPreset)
    {
        if (_themeDict is null) return;

        // Remove old preset from the theme dictionary
        if (_activePresetDict is not null)
        {
            _themeDict.MergedDictionaries.Remove(_activePresetDict);
        }

        // Remove runtime overrides temporarily (they must stay last = highest priority)
        _themeDict.MergedDictionaries.Remove(_runtimeOverrides);

        // Add new preset
        _activePresetDict = newPreset;
        _themeDict.MergedDictionaries.Add(_activePresetDict);

        // Re-add overrides last
        _themeDict.MergedDictionaries.Add(_runtimeOverrides);
    }

    /// <summary>
    /// Searches <paramref name="target"/>'s MergedDictionaries for an already-loaded
    /// FluentDialogs.Theme.xaml (e.g., merged from App.xaml at design time).
    /// </summary>
    private static ResourceDictionary? FindExistingThemeDict(ResourceDictionary target)
    {
        foreach (var dict in target.MergedDictionaries)
        {
            if (dict.Source is { } uri && uri.OriginalString.Contains(ThemeEntryFileName, StringComparison.OrdinalIgnoreCase))
            {
                return dict;
            }
        }
        return null;
    }

    private void ApplyAccentColorInternal(Color accent)
    {
        // Derive hover and pressed states from the accent color
        var hover = DarkenColor(accent, 0.15);
        var pressed = DarkenColor(accent, 0.30);

        _runtimeOverrides[ThemeTokenKeys.InteractiveDefault] = accent;
        _runtimeOverrides[ThemeTokenKeys.InteractiveHover] = hover;
        _runtimeOverrides[ThemeTokenKeys.InteractivePressed] = pressed;
        _runtimeOverrides[ThemeTokenKeys.LinkDefault] = accent;
        _runtimeOverrides[ThemeTokenKeys.LinkHover] = hover;
    }

    private static Color DarkenColor(Color color, double factor)
    {
        var scale = 1.0 - Math.Clamp(factor, 0, 1);
        return Color.FromArgb(
            color.A,
            (byte)(color.R * scale),
            (byte)(color.G * scale),
            (byte)(color.B * scale));
    }

    private static Uri GetBuiltInPresetUri(MessageBoxTheme preset) => preset switch
    {
        MessageBoxTheme.Dark => DarkPresetUri,
        _ => LightPresetUri
    };

    private void EnsureLoaded()
    {
        if (!_isLoaded)
        {
            EnsureThemeLoaded();
        }
    }

    private static void VerifyUIThread()
    {
        if (Application.Current?.Dispatcher is { } dispatcher &&
            !dispatcher.CheckAccess())
        {
            throw new InvalidOperationException(
                "FluentDialogThemeService must be called from the UI thread. " +
                "Use Dispatcher.Invoke() to marshal the call.");
        }
    }
}
