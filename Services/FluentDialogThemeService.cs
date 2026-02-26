using System;
using System.Collections.Generic;
using System.Diagnostics;
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
public sealed class FluentDialogThemeService : IFluentDialogThemeService
{
    private static readonly Uri LightPresetUri =
        new("pack://application:,,,/FluentDialogs.Wpf;component/Themes/Presets/Light.xaml", UriKind.Absolute);

    private static readonly Uri DarkPresetUri =
        new("pack://application:,,,/FluentDialogs.Wpf;component/Themes/Presets/Dark.xaml", UriKind.Absolute);

    private static readonly Uri ThemeEntryUri =
        new("pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentDialogs.Theme.xaml", UriKind.Absolute);

    private readonly FluentDialogOptions _options;
    private readonly ResourceDictionary _runtimeOverrides = new();
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

        // 1. Load the single entry-point theme dictionary
        var themeDict = new ResourceDictionary { Source = ThemeEntryUri };
        _target.MergedDictionaries.Add(themeDict);

        // 2. Apply the configured default preset
        var presetUri = _options.CustomPresetUri ?? GetBuiltInPresetUri(_options.DefaultPreset);
        _activePresetDict = new ResourceDictionary { Source = presetUri };
        _target.MergedDictionaries.Add(_activePresetDict);

        // 3. Add runtime overrides dictionary (always last)
        _target.MergedDictionaries.Add(_runtimeOverrides);

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

    private void SwapPreset(ResourceDictionary newPreset)
    {
        if (_target is null) return;

        // Remove old preset
        if (_activePresetDict is not null)
        {
            _target.MergedDictionaries.Remove(_activePresetDict);
        }

        // Remove runtime overrides temporarily (they must stay last)
        _target.MergedDictionaries.Remove(_runtimeOverrides);

        // Add new preset
        _activePresetDict = newPreset;
        _target.MergedDictionaries.Add(_activePresetDict);

        // Re-add overrides last
        _target.MergedDictionaries.Add(_runtimeOverrides);
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
