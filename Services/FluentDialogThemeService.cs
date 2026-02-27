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
/// <para>
/// Presets and runtime overrides are injected into <c>FluentDialogs.Theme.xaml</c>'s own
/// <see cref="ResourceDictionary.MergedDictionaries"/>.
/// </para>
/// <para>
/// <b>Important WPF limitation:</b> <c>DynamicResource</c> expressions on <c>SolidColorBrush.Color</c>
/// inside a <see cref="ResourceDictionary"/> do NOT re-evaluate when the referenced Color resource changes,
/// because <c>Freezable</c> objects without a visual-tree mentor don't receive resource-change notifications.
/// To work around this, <see cref="SyncBrushColors"/> is called after every mutation to programmatically
/// push resolved Color values onto each brush object.
/// </para>
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

    /// <summary>
    /// Complete mapping of every SolidColorBrush key → its corresponding semantic Color key.
    /// Covers both the v2 FDBrush* layer and the v1-compat legacy brush aliases.
    /// </summary>
    private static readonly (string BrushKey, string ColorKey)[] BrushColorMappings =
    [
        // ── v2 public API brushes (_Brushes.xaml) ──
        (ThemeTokenKeys.Brushes.SurfacePrimary, ThemeTokenKeys.SurfacePrimary),
        (ThemeTokenKeys.Brushes.SurfaceSecondary, ThemeTokenKeys.SurfaceSecondary),
        (ThemeTokenKeys.Brushes.SurfaceOverlay, ThemeTokenKeys.SurfaceOverlay),
        (ThemeTokenKeys.Brushes.OnSurfacePrimary, ThemeTokenKeys.OnSurfacePrimary),
        (ThemeTokenKeys.Brushes.OnSurfaceSecondary, ThemeTokenKeys.OnSurfaceSecondary),
        (ThemeTokenKeys.Brushes.InteractiveDefault, ThemeTokenKeys.InteractiveDefault),
        (ThemeTokenKeys.Brushes.InteractiveHover, ThemeTokenKeys.InteractiveHover),
        (ThemeTokenKeys.Brushes.InteractivePressed, ThemeTokenKeys.InteractivePressed),
        (ThemeTokenKeys.Brushes.OnInteractive, ThemeTokenKeys.OnInteractive),
        (ThemeTokenKeys.Brushes.NeutralDefault, ThemeTokenKeys.NeutralDefault),
        (ThemeTokenKeys.Brushes.NeutralHover, ThemeTokenKeys.NeutralHover),
        (ThemeTokenKeys.Brushes.NeutralPressed, ThemeTokenKeys.NeutralPressed),
        (ThemeTokenKeys.Brushes.StatusError, ThemeTokenKeys.StatusError),
        (ThemeTokenKeys.Brushes.StatusErrorSubtle, ThemeTokenKeys.StatusErrorSubtle),
        (ThemeTokenKeys.Brushes.OnStatusError, ThemeTokenKeys.OnStatusError),
        (ThemeTokenKeys.Brushes.StatusErrorHover, ThemeTokenKeys.StatusErrorHover),
        (ThemeTokenKeys.Brushes.StatusErrorPressed, ThemeTokenKeys.StatusErrorPressed),
        (ThemeTokenKeys.Brushes.StatusWarning, ThemeTokenKeys.StatusWarning),
        (ThemeTokenKeys.Brushes.StatusWarningSubtle, ThemeTokenKeys.StatusWarningSubtle),
        (ThemeTokenKeys.Brushes.StatusSuccess, ThemeTokenKeys.StatusSuccess),
        (ThemeTokenKeys.Brushes.StatusSuccessSubtle, ThemeTokenKeys.StatusSuccessSubtle),
        (ThemeTokenKeys.Brushes.StatusInfo, ThemeTokenKeys.StatusInfo),
        (ThemeTokenKeys.Brushes.BorderDefault, ThemeTokenKeys.BorderDefault),
        (ThemeTokenKeys.Brushes.BorderStrong, ThemeTokenKeys.BorderStrong),
        (ThemeTokenKeys.Brushes.Shadow, ThemeTokenKeys.Shadow),
        (ThemeTokenKeys.Brushes.LinkDefault, ThemeTokenKeys.LinkDefault),
        (ThemeTokenKeys.Brushes.LinkHover, ThemeTokenKeys.LinkHover),
        (ThemeTokenKeys.Brushes.CloseHover, ThemeTokenKeys.CloseHover),
        (ThemeTokenKeys.Brushes.ClosePressed, ThemeTokenKeys.ClosePressed),
        (ThemeTokenKeys.Brushes.OnClose, ThemeTokenKeys.OnClose),

        // ── v1-compat legacy brush aliases (Legacy/v1-compat.xaml) ──
        ("DialogBackgroundBrush", ThemeTokenKeys.SurfacePrimary),
        ("DialogOverlayBrush", ThemeTokenKeys.SurfaceOverlay),
        ("DialogForegroundBrush", ThemeTokenKeys.OnSurfacePrimary),
        ("DialogSecondaryForegroundBrush", ThemeTokenKeys.OnSurfaceSecondary),
        ("DialogBorderBrush", ThemeTokenKeys.BorderDefault),
        ("DialogShadowBrush", ThemeTokenKeys.Shadow),
        ("ButtonBackgroundBrush", ThemeTokenKeys.NeutralDefault),
        ("ButtonForegroundBrush", ThemeTokenKeys.OnSurfacePrimary),
        ("ButtonBorderBrush", ThemeTokenKeys.BorderDefault),
        ("ButtonHoverBackgroundBrush", ThemeTokenKeys.NeutralHover),
        ("ButtonHoverBorderBrush", ThemeTokenKeys.BorderStrong),
        ("ButtonPressedBackgroundBrush", ThemeTokenKeys.NeutralPressed),
        ("PrimaryButtonBackgroundBrush", ThemeTokenKeys.InteractiveDefault),
        ("PrimaryButtonForegroundBrush", ThemeTokenKeys.OnInteractive),
        ("PrimaryButtonHoverBackgroundBrush", ThemeTokenKeys.InteractiveHover),
        ("PrimaryButtonPressedBackgroundBrush", ThemeTokenKeys.InteractivePressed),
        ("DangerButtonBackgroundBrush", ThemeTokenKeys.StatusError),
        ("DangerButtonForegroundBrush", ThemeTokenKeys.OnInteractive),
        ("DangerButtonHoverBackgroundBrush", ThemeTokenKeys.StatusErrorHover),
        ("DangerButtonPressedBackgroundBrush", ThemeTokenKeys.StatusErrorPressed),
        ("ExceptionBackgroundBrush", ThemeTokenKeys.StatusErrorSubtle),
        ("ExceptionBorderBrush", ThemeTokenKeys.OnStatusError),
        ("ExceptionForegroundBrush", ThemeTokenKeys.OnStatusError),
        ("HyperlinkBrush", ThemeTokenKeys.LinkDefault),
        ("HyperlinkHoverBrush", ThemeTokenKeys.LinkHover),
        ("ErrorDialogBorderBrush", ThemeTokenKeys.StatusError),
        ("ErrorDialogAccentBrush", ThemeTokenKeys.StatusErrorSubtle),
        ("WarningDialogBorderBrush", ThemeTokenKeys.StatusWarning),
        ("WarningDialogAccentBrush", ThemeTokenKeys.StatusWarningSubtle),
        ("DialogBackground", ThemeTokenKeys.SurfacePrimary),
        ("DialogForeground", ThemeTokenKeys.OnSurfacePrimary),
        ("DialogSecondaryForeground", ThemeTokenKeys.OnSurfaceSecondary),
    ];

    private readonly FluentDialogOptions _options;
    private readonly ResourceDictionary _runtimeOverrides = new();

    /// <summary>
    /// The FluentDialogs.Theme.xaml dictionary instance.
    /// Presets and runtime overrides are added to THIS dictionary's MergedDictionaries.
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

        // 6. Push resolved Color values onto every brush (WPF Freezable workaround)
        SyncBrushColors();

        // 7. Validate in debug builds
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

        SyncBrushColors();

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

        SyncBrushColors();

        PresetChanged?.Invoke(this, new ThemePresetChangedEventArgs(oldPreset, CurrentPreset, presetName));
    }

    /// <inheritdoc/>
    public void SetToken(string tokenKey, Color color)
    {
        VerifyUIThread();
        EnsureLoaded();

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenKey);
        _runtimeOverrides[tokenKey] = color;

        SyncBrushColors();
    }

    /// <inheritdoc/>
    public void SetAccentColor(Color accentColor)
    {
        VerifyUIThread();
        EnsureLoaded();

        ApplyAccentColorInternal(accentColor);
        SyncBrushColors();
    }

    /// <inheritdoc/>
    public void ClearOverrides()
    {
        VerifyUIThread();
        EnsureLoaded();

        _runtimeOverrides.Clear();
        SyncBrushColors();
    }

    // ═══════════════ Private Helpers ═══════════════

    /// <summary>
    /// Swaps the active preset dictionary inside <see cref="_themeDict"/>.
    /// Runtime overrides are removed/re-added to maintain highest priority (last position).
    /// </summary>
    private void SwapPreset(ResourceDictionary newPreset)
    {
        if (_themeDict is null) return;

        if (_activePresetDict is not null)
        {
            _themeDict.MergedDictionaries.Remove(_activePresetDict);
        }

        _themeDict.MergedDictionaries.Remove(_runtimeOverrides);

        _activePresetDict = newPreset;
        _themeDict.MergedDictionaries.Add(_activePresetDict);

        _themeDict.MergedDictionaries.Add(_runtimeOverrides);
    }

    /// <summary>
    /// Pushes resolved semantic Color values onto every known SolidColorBrush.
    /// </summary>
    /// <remarks>
    /// WPF <c>DynamicResource</c> expressions on <c>Freezable.Color</c> inside a
    /// <see cref="ResourceDictionary"/> resolve once during XAML load but never re-evaluate
    /// when the referenced Color resource changes (because Freezables without a visual-tree
    /// mentor don't receive resource-change notifications). This method works around that
    /// limitation by reading the current Color value from the dictionary tree and directly
    /// setting it on each brush's <see cref="SolidColorBrush.Color"/> property.
    /// </remarks>
    /// <summary>
    /// Pushes resolved semantic Color values onto every known SolidColorBrush.
    /// </summary>
    /// <remarks>
    /// <para>
    /// WPF <c>DynamicResource</c> expressions on <c>Freezable.Color</c> inside a
    /// <see cref="ResourceDictionary"/> resolve once during XAML load but never re-evaluate
    /// when the referenced Color resource changes (Freezables without a visual-tree
    /// mentor don't receive resource-change notifications).
    /// </para>
    /// <para>
    /// Additionally, brushes loaded from compiled BAML (pack URI) are <b>automatically frozen</b>
    /// by WPF — their <see cref="System.Windows.Freezable.IsFrozen"/> is <c>true</c> and setting
    /// <c>Color</c> throws. This method handles both cases:
    /// <list type="bullet">
    ///   <item>Frozen brush → replaced with a new unfrozen <see cref="SolidColorBrush"/> in
    ///         <c>_themeDict</c> (direct entries shadow MergedDictionaries; DynamicResource
    ///         holders re-resolve automatically).</item>
    ///   <item>Unfrozen brush (already replaced on a prior call) → <c>Color</c> updated in place
    ///         (DependencyProperty change notification triggers re-render).</item>
    /// </list>
    /// </para>
    /// </remarks>
    private void SyncBrushColors()
    {
        if (_themeDict is null) return;

        foreach (var (brushKey, colorKey) in BrushColorMappings)
        {
            // Resolve the semantic color (searches MergedDictionaries last-to-first:
            // _runtimeOverrides → _activePresetDict → v1-compat → … → _Semantics)
            if (_themeDict[colorKey] is not Color resolvedColor)
                continue;

            if (_themeDict[brushKey] is SolidColorBrush brush)
            {
                if (brush.IsFrozen)
                {
                    // BAML-loaded brush is frozen — replace with an unfrozen copy.
                    // Direct entries in _themeDict shadow the frozen original in
                    // MergedDictionaries; DynamicResource holders re-resolve.
                    _themeDict[brushKey] = new SolidColorBrush(resolvedColor);
                }
                else
                {
                    // Already unfrozen (replaced on a prior call) — update in place
                    brush.Color = resolvedColor;
                }
            }
            else
            {
                // Brush doesn't exist yet — create it
                _themeDict[brushKey] = new SolidColorBrush(resolvedColor);
            }
        }
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
