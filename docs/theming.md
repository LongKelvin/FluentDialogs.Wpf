# Theming Guide

Customize the appearance of FluentDialogs.Wpf dialogs and toasts using the v2 design token system.

## Overview

FluentDialogs uses a **three-layer design token** architecture inspired by modern design systems:

```
┌─────────────────────────────────────────────────┐
│  Primitives  (_Primitives.xaml)                  │
│  Raw color palette — never referenced by controls│
├─────────────────────────────────────────────────┤
│  Semantics   (_Semantics.xaml)                   │
│  Meaning-based aliases — THE customization layer │
├─────────────────────────────────────────────────┤
│  Brushes     (_Brushes.xaml)                     │
│  SolidColorBrush resources consumed by controls  │
└─────────────────────────────────────────────────┘
```

Controls only reference **Brush** tokens (`FDBrush*`). Brushes bind to **Semantic** tokens (`FDSem*`). Semantics reference **Primitive** colors (`FDColor*`). To customize the look, you override the semantic layer — everything downstream updates automatically.

## Setting Up Themes

### 1. Merge Theme Resources

In `App.xaml`, merge the single entry point:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentDialogs.Theme.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

`FluentDialogs.Theme.xaml` loads all layers in the correct order:

1. `_Primitives.xaml` — Raw color palette
2. `_Semantics.xaml` — Meaning-based defaults (light)
3. `_Brushes.xaml` — SolidColorBrush public API
4. `_Typography.xaml` — Font families, sizes, weights
5. `_Layout.xaml` — Spacing, radii, sizing, elevation
6. `_Infrastructure.xaml` — Converters (BoolToVisibility, etc.)
7. `Controls/*.xaml` — Button, dialog, progress, toast, input, icon styles
8. `Legacy/v1-compat.xaml` — Backward compatibility with v1 keys
9. `Presets/Light.xaml` — Default light preset (loaded last to override semantics)

### 2. Register Services

```csharp
services.AddFluentDialogs(options =>
{
    options.DefaultPreset = MessageBoxTheme.Light; // or .Dark
});
```

---

## Switching Themes at Runtime

### Using IFluentDialogThemeService (v2)

Inject `IFluentDialogThemeService` — the primary theme management interface:

```csharp
public class SettingsViewModel
{
    private readonly IFluentDialogThemeService _theme;

    public SettingsViewModel(IFluentDialogThemeService theme)
    {
        _theme = theme;
    }

    public bool IsDarkMode
    {
        get => _theme.CurrentPreset == MessageBoxTheme.Dark;
        set => _theme.ApplyPreset(value ? MessageBoxTheme.Dark : MessageBoxTheme.Light);
    }
}
```

### Using IMessageBoxThemeService (v1 Legacy)

The v1 interface still works via a built-in legacy adapter:

```csharp
// Still works — routes through the v2 service internally
_themeService.SetTheme(MessageBoxTheme.Dark);
```

---

## Preset Changed Event

React to theme preset changes across your application:

```csharp
public class AppViewModel : IDisposable
{
    private readonly IFluentDialogThemeService _theme;

    public AppViewModel(IFluentDialogThemeService theme)
    {
        _theme = theme;
        _theme.PresetChanged += OnPresetChanged;
    }

    private void OnPresetChanged(object? sender, ThemePresetChangedEventArgs e)
    {
        Console.WriteLine($"Theme changed from {e.OldPreset} to {e.NewPreset}");

        // Update app-specific theme elements
        UpdateApplicationTheme(e.NewPreset);

        // Persist preference
        Settings.Default.Theme = e.NewPreset.ToString();
        Settings.Default.Save();
    }

    public void Dispose()
    {
        _theme.PresetChanged -= OnPresetChanged;
    }
}
```

---

## Accent / Brand Color

Set a custom accent color that automatically derives hover and pressed states:

```csharp
services.AddFluentDialogs(options =>
{
    options.AccentColor = Color.FromRgb(0, 120, 212); // Your brand color
});
```

Or at runtime:

```csharp
_theme.SetAccentColor(Color.FromRgb(128, 0, 128)); // Purple brand
```

This overrides the following semantic tokens:
- `FDSemInteractiveDefault` — The accent color
- `FDSemInteractiveHover` — 15% darker
- `FDSemInteractivePressed` — 25% darker
- `FDSemLinkDefault` — Same as accent
- `FDSemLinkHover` — Same as hover

---

## Custom Title Bar Color

Override the title bar color per dialog:

```csharp
var options = new MessageBoxOptions
{
    Title = "Brand Dialog",
    Message = "This dialog uses your brand color.",
    Buttons = MessageBoxButtons.OK,
    TitleBarColor = Color.FromRgb(0, 120, 212)
};

await _messageBox.ShowAsync(options);
```

The text color automatically adjusts for contrast (light text on dark backgrounds, dark text on light backgrounds).

---

## Customizing with Token Overrides

### At Registration Time

Override individual tokens when configuring the service:

```csharp
services.AddFluentDialogs(options =>
{
    options.DefaultPreset = MessageBoxTheme.Light;

    // Override specific tokens
    options.TokenOverrides[ThemeTokenKeys.InteractiveDefault] = Colors.Purple;
    options.TokenOverrides[ThemeTokenKeys.StatusSuccess] = Color.FromRgb(0, 168, 107);
});
```

### At Runtime

Override tokens dynamically via C#:

```csharp
// Override a single token
_theme.SetToken(ThemeTokenKeys.InteractiveDefault, Colors.DarkOrange);

// Clear all overrides (revert to preset values)
_theme.ClearOverrides();
```

### In XAML (App.xaml)

Override semantic tokens after merging the theme entry point:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentDialogs.Theme.xaml"/>
        </ResourceDictionary.MergedDictionaries>

        <!-- Override semantic tokens directly -->
        <Color x:Key="FDSemInteractiveDefault">#00A86B</Color>
        <Color x:Key="FDSemInteractiveHover">#008F5A</Color>
        <Color x:Key="FDSemInteractivePressed">#007A4D</Color>
        <Color x:Key="FDSemStatusError">#C42B1C</Color>
    </ResourceDictionary>
</Application.Resources>
```

---

## Semantic Token Reference

All `FDSem*` tokens and their purpose. Override these to customize the look.

### Surface

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemSurfacePrimary` | Dialog/toast backgrounds | `#F3F3F3` | `#2D2D2D` |
| `FDSemSurfaceSecondary` | Secondary panels, inputs | `#FAFAFA` | `#383838` |
| `FDSemSurfaceOverlay` | Overlay/shadow backgrounds | `#20000000` | `#40000000` |

### On Surface (Text)

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemOnSurfacePrimary` | Primary text | `#1B1B1B` | `#F5F5F5` |
| `FDSemOnSurfaceSecondary` | Secondary/hint text | `#605E5C` | `#C8C6C4` |

### Interactive (Accent / Primary Buttons)

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemInteractiveDefault` | Primary button background, links | `#005FB8` | `#60CDFF` |
| `FDSemInteractiveHover` | Interactive hover state | `#004E99` | `#4DC4FF` |
| `FDSemInteractivePressed` | Interactive pressed state | `#003E7E` | `#3ABCFF` |
| `FDSemOnInteractive` | Text on interactive surfaces | `#FFFFFF` | `#003E7E` |

### Neutral (Default Buttons)

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemNeutralDefault` | Default button background | `#FDFDFD` | `#454545` |
| `FDSemNeutralHover` | Default button hover | `#F5F5F5` | `#505050` |
| `FDSemNeutralPressed` | Default button pressed | `#E8E8E8` | `#3D3D3D` |

### Status Colors

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemStatusError` | Error/danger color | `#C42B1C` | `#FF99A4` |
| `FDSemStatusErrorSubtle` | Error background tint | `#FDE7E9` | `#442726` |
| `FDSemOnStatusError` | Text on error surfaces | `#FFFFFF` | `#1B1B1B` |
| `FDSemStatusErrorHover` | Error hover state | `#A31A0E` | `#E68790` |
| `FDSemStatusErrorPressed` | Error pressed state | `#8A130A` | `#CC767E` |
| `FDSemStatusWarning` | Warning color | `#F7630C` | `#FCB828` |
| `FDSemStatusWarningSubtle` | Warning background tint | `#FFF4CE` | `#433519` |
| `FDSemStatusSuccess` | Success color | `#0F7B0F` | `#6CCB5F` |
| `FDSemStatusSuccessSubtle` | Success background tint | `#DFF6DD` | `#393D1B` |
| `FDSemStatusInfo` | Info color | `#005FB8` | `#60CDFF` |

### Border & Shadow

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemBorderDefault` | Standard borders | `#E1DFDD` | `#3E3E3E` |
| `FDSemBorderStrong` | Emphasized borders | `#8A8886` | `#6E6E6E` |
| `FDSemShadow` | Drop shadow color | `#29000000` | `#47000000` |

### Link

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemLinkDefault` | Hyperlink color | `#005FB8` | `#60CDFF` |
| `FDSemLinkHover` | Hyperlink hover | `#004E99` | `#4DC4FF` |

### Close Button

| Token Key | Description | Light Default | Dark Default |
|-----------|-------------|---------------|--------------|
| `FDSemCloseHover` | Close button hover background | `#E81123` | `#E81123` |
| `FDSemClosePressed` | Close button pressed background | `#F1707A` | `#C50F1F` |
| `FDSemOnClose` | Close button hover/pressed text | `#FFFFFF` | `#FFFFFF` |

---

## Creating a Custom Preset

Create a complete custom preset by defining all `FDSem*` keys in a ResourceDictionary.

### MyBrandPreset.xaml

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Surface -->
    <Color x:Key="FDSemSurfacePrimary">#1A1A2E</Color>
    <Color x:Key="FDSemSurfaceSecondary">#22223A</Color>
    <Color x:Key="FDSemSurfaceOverlay">#40000000</Color>

    <!-- Text -->
    <Color x:Key="FDSemOnSurfacePrimary">#EAEAEA</Color>
    <Color x:Key="FDSemOnSurfaceSecondary">#A0A0B0</Color>

    <!-- Interactive / Accent -->
    <Color x:Key="FDSemInteractiveDefault">#E94560</Color>
    <Color x:Key="FDSemInteractiveHover">#D63850</Color>
    <Color x:Key="FDSemInteractivePressed">#C02040</Color>
    <Color x:Key="FDSemOnInteractive">#FFFFFF</Color>

    <!-- Neutral -->
    <Color x:Key="FDSemNeutralDefault">#2A2A4E</Color>
    <Color x:Key="FDSemNeutralHover">#3A3A6E</Color>
    <Color x:Key="FDSemNeutralPressed">#1A1A2E</Color>

    <!-- Status -->
    <Color x:Key="FDSemStatusError">#FF6B6B</Color>
    <Color x:Key="FDSemStatusErrorSubtle">#3D2020</Color>
    <Color x:Key="FDSemOnStatusError">#FFFFFF</Color>
    <Color x:Key="FDSemStatusErrorHover">#E55A5A</Color>
    <Color x:Key="FDSemStatusErrorPressed">#CC4A4A</Color>
    <Color x:Key="FDSemStatusWarning">#FCB828</Color>
    <Color x:Key="FDSemStatusWarningSubtle">#3D3519</Color>
    <Color x:Key="FDSemStatusSuccess">#6CCB5F</Color>
    <Color x:Key="FDSemStatusSuccessSubtle">#2D3D1B</Color>
    <Color x:Key="FDSemStatusInfo">#60CDFF</Color>

    <!-- Border & Shadow -->
    <Color x:Key="FDSemBorderDefault">#2A2A4E</Color>
    <Color x:Key="FDSemBorderStrong">#4A4A7E</Color>
    <Color x:Key="FDSemShadow">#60000000</Color>

    <!-- Link -->
    <Color x:Key="FDSemLinkDefault">#E94560</Color>
    <Color x:Key="FDSemLinkHover">#D63850</Color>

    <!-- Close Button -->
    <Color x:Key="FDSemCloseHover">#E81123</Color>
    <Color x:Key="FDSemClosePressed">#C50F1F</Color>
    <Color x:Key="FDSemOnClose">#FFFFFF</Color>
</ResourceDictionary>
```

### Apply Custom Preset

#### At Registration

```csharp
services.AddFluentDialogs(options =>
{
    options.CustomPresetUri = new Uri(
        "pack://application:,,,/YourApp;component/Themes/MyBrandPreset.xaml");
});
```

#### At Runtime

```csharp
_theme.ApplyCustomPreset(
    new Uri("pack://application:,,,/YourApp;component/Themes/MyBrandPreset.xaml"),
    "My Brand");
```

---

## Theme Synchronization

### Sync with System Theme

Detect and sync with the Windows system theme:

```csharp
using Microsoft.Win32;

public class ThemeManager
{
    private readonly IFluentDialogThemeService _theme;

    public ThemeManager(IFluentDialogThemeService theme)
    {
        _theme = theme;

        // Apply initial theme
        ApplySystemTheme();

        // Listen for system theme changes
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            ApplySystemTheme();
        }
    }

    private void ApplySystemTheme()
    {
        var isDark = IsSystemDarkTheme();
        _theme.ApplyPreset(isDark ? MessageBoxTheme.Dark : MessageBoxTheme.Light);
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int intValue && intValue == 0;
        }
        catch
        {
            return false;
        }
    }
}
```

### Persist Theme Preference

```csharp
public class SettingsViewModel
{
    private readonly IFluentDialogThemeService _theme;

    public SettingsViewModel(IFluentDialogThemeService theme)
    {
        _theme = theme;

        // Load saved preference
        if (Enum.TryParse<MessageBoxTheme>(Settings.Default.Theme, out var preset))
        {
            _theme.ApplyPreset(preset);
        }

        // Save on change
        _theme.PresetChanged += (_, e) =>
        {
            Settings.Default.Theme = e.NewPreset.ToString();
            Settings.Default.Save();
        };
    }
}
```

---

## Token Architecture Deep Dive

### Naming Convention

All resource keys follow the pattern: `FD{Layer}{Category}{Variant}{State}`

| Prefix | Layer | Purpose |
|--------|-------|---------|
| `FDColor` | Primitive | Raw palette values (Neutral000–1000, Accent100–600, etc.) |
| `FDSem` | Semantic | Meaning-based aliases — the customization surface |
| `FDBrush` | Brush | `SolidColorBrush` resources consumed by styles |
| `FDType` | Typography | Font family, size, weight |
| `FDRadius` | Layout | Corner radii |
| `FDSpacing` | Layout | Margins and padding |
| `FDSize` | Layout | Component sizing |

### How Layers Connect

```xml
<!-- _Primitives.xaml — raw value -->
<Color x:Key="FDColorAccent400">#005FB8</Color>

<!-- _Semantics.xaml — meaning -->
<Color x:Key="FDSemInteractiveDefault">#005FB8</Color>

<!-- _Brushes.xaml — brush that controls consume -->
<SolidColorBrush x:Key="FDBrushInteractiveDefault"
                 Color="{DynamicResource FDSemInteractiveDefault}"/>
```

When you override `FDSemInteractiveDefault`, the brush updates automatically because it uses `DynamicResource`.

### ThemeTokenKeys Constants

Use `ThemeTokenKeys` from C# to avoid magic strings:

```csharp
using FluentDialogs;

// Semantic keys
_theme.SetToken(ThemeTokenKeys.InteractiveDefault, Colors.Purple);
_theme.SetToken(ThemeTokenKeys.StatusError, Color.FromRgb(220, 50, 50));

// Brush keys (for FindResource)
var brush = (SolidColorBrush)FindResource(ThemeTokenKeys.Brushes.InteractiveDefault);
```

---

## Disabling Legacy Compatibility

By default, `FluentDialogs.Theme.xaml` includes `Legacy/v1-compat.xaml` which maps old v1 keys (`DialogBackgroundColor`, `PrimaryButtonBackgroundColor`, etc.) to the v2 token system. To opt out for a smaller resource tree:

```csharp
services.AddFluentDialogs(options =>
{
    options.IncludeLegacyCompatibility = false;
});
```

> **Warning:** Only set this to `false` if you have no code or XAML referencing v1 resource keys.

---

## v1 → v2 Key Migration Reference

If migrating custom theme overrides from v1 to v2:

| v1 Key | v2 Semantic Token |
|--------|-------------------|
| `DialogBackgroundColor` | `FDSemSurfacePrimary` |
| `DialogForegroundColor` | `FDSemOnSurfacePrimary` |
| `DialogSecondaryForegroundColor` | `FDSemOnSurfaceSecondary` |
| `DialogBorderColor` | `FDSemBorderDefault` |
| `PrimaryButtonBackgroundColor` | `FDSemInteractiveDefault` |
| `PrimaryButtonHoverBackgroundColor` | `FDSemInteractiveHover` |
| `PrimaryButtonPressedBackgroundColor` | `FDSemInteractivePressed` |
| `PrimaryButtonForegroundColor` | `FDSemOnInteractive` |
| `ButtonBackgroundColor` | `FDSemNeutralDefault` |
| `ButtonHoverBackgroundColor` | `FDSemNeutralHover` |
| `ButtonPressedBackgroundColor` | `FDSemNeutralPressed` |
| `DangerButtonBackgroundColor` | `FDSemStatusError` |
| `DangerButtonHoverBackgroundColor` | `FDSemStatusErrorHover` |
| `DangerButtonPressedBackgroundColor` | `FDSemStatusErrorPressed` |
| `InfoIconColor` | `FDSemStatusInfo` |
| `WarningIconColor` | `FDSemStatusWarning` |
| `ErrorIconColor` | `FDSemStatusError` |
| `SuccessIconColor` | `FDSemStatusSuccess` |
| `ToastBackgroundColor` | `FDSemSurfacePrimary` |
| `ToastForegroundColor` | `FDSemOnSurfacePrimary` |

---

## Best Practices

1. **Merge only `FluentDialogs.Theme.xaml`** — It handles all internal dependencies and ordering
2. **Override semantics, not brushes** — Override `FDSem*` Color tokens; brushes update automatically via DynamicResource
3. **Use `ThemeTokenKeys` constants** — Avoid magic strings in C# when calling `SetToken()`
4. **Test both presets** — Ensure custom overrides look correct in both Light and Dark modes
5. **Consider accessibility** — Ensure sufficient contrast ratios (4.5:1 for text, 3:1 for large text)
6. **Keep presets complete** — When creating a custom preset, define ALL `FDSem*` keys (use `Presets/Light.xaml` as a template)
7. **Use accent color for branding** — `SetAccentColor()` or `options.AccentColor` is the simplest way to apply a brand color
