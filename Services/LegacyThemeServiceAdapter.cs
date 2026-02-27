using System;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.Services;

/// <summary>
/// Adapts the v2 <see cref="IFluentDialogThemeService"/> to the v1 <see cref="IMessageBoxThemeService"/> interface.
/// Allows existing consumer code that depends on <see cref="IMessageBoxThemeService"/> to continue working.
/// </summary>
[Obsolete("Use IFluentDialogThemeService instead. This adapter will be removed in v3.0.")]
public sealed class LegacyThemeServiceAdapter : IMessageBoxThemeService
{
    private readonly IFluentDialogThemeService _inner;

    public LegacyThemeServiceAdapter(IFluentDialogThemeService inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));

        // Forward preset changes to the legacy ThemeChanged event
        _inner.PresetChanged += (_, e) =>
        {
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(e.OldPreset, e.NewPreset));
        };
    }

    /// <inheritdoc/>
    public MessageBoxTheme CurrentTheme => _inner.CurrentPreset;

    /// <inheritdoc/>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <inheritdoc/>
    public void SetTheme(MessageBoxTheme theme)
    {
        _inner.ApplyPreset(theme);
    }
}
