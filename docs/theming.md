# Theming Guide

Customize the appearance of FluentDialogs.Wpf dialogs and toasts.

## Built-in Themes

FluentDialogs ships with two themes matching Windows 11 Fluent Design:

- **Light** - Default light theme
- **Dark** - Dark theme

## Setting Up Themes

### 1. Merge Theme Resources

In `App.xaml`, merge the required resources:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- Required: Base resources with converters and styles -->
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/ThemeResources.xaml"/>
            
            <!-- Initial theme (Light or Dark) -->
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentLight.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 2. Switch Themes at Runtime

Inject `IMessageBoxThemeService` to switch themes:

```csharp
public class SettingsViewModel
{
    private readonly IMessageBoxThemeService _themeService;

    public SettingsViewModel(IMessageBoxThemeService themeService)
    {
        _themeService = themeService;
    }

    public bool IsDarkMode
    {
        get => _themeService.CurrentTheme == MessageBoxTheme.Dark;
        set => _themeService.SetTheme(value ? MessageBoxTheme.Dark : MessageBoxTheme.Light);
    }
}
```

---

## Theme Changed Event

React to theme changes across your application:

```csharp
public class AppViewModel : IDisposable
{
    private readonly IMessageBoxThemeService _themeService;

    public AppViewModel(IMessageBoxThemeService themeService)
    {
        _themeService = themeService;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        Console.WriteLine($"Theme changed from {e.OldTheme} to {e.NewTheme}");
        
        // Update app-specific theme elements
        UpdateApplicationTheme(e.NewTheme);
        
        // Persist preference
        Settings.Default.Theme = e.NewTheme.ToString();
        Settings.Default.Save();
    }

    public void Dispose()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
    }
}
```

---

## Custom Title Bar Color

Override the title bar color per dialog:

```csharp
using System.Windows.Media;

var options = new MessageBoxOptions
{
    Title = "Brand Dialog",
    Message = "This dialog uses your brand color.",
    Buttons = MessageBoxButtons.OK,
    TitleBarColor = Color.FromRgb(0, 120, 212)  // Your brand color
};

await _messageBox.ShowAsync(options);
```

The text color automatically adjusts for contrast (light text on dark backgrounds, dark text on light backgrounds).

---

## Customizing Colors

Override theme colors by redefining resource keys after merging the base theme.

### In App.xaml

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/ThemeResources.xaml"/>
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentLight.xaml"/>
        </ResourceDictionary.MergedDictionaries>
        
        <!-- Override primary button color -->
        <Color x:Key="PrimaryButtonBackgroundColor">#00A86B</Color>
        <Color x:Key="PrimaryButtonHoverBackgroundColor">#008F5A</Color>
        <Color x:Key="PrimaryButtonPressedBackgroundColor">#007A4D</Color>
        
        <!-- Override danger button color -->
        <Color x:Key="DangerButtonBackgroundColor">#C42B1C</Color>
    </ResourceDictionary>
</Application.Resources>
```

### Available Color Keys

#### Dialog Colors

| Key | Description | Light Default | Dark Default |
|-----|-------------|---------------|--------------|
| `DialogBackgroundColor` | Dialog background | `#F3F3F3` | `#2D2D2D` |
| `DialogForegroundColor` | Primary text | `#000000` | `#FFFFFF` |
| `DialogSecondaryForegroundColor` | Secondary text | `#605E5C` | `#C8C6C4` |
| `DialogBorderColor` | Dialog border | `#E1DFDD` | `#3E3E3E` |

#### Button Colors

| Key | Description |
|-----|-------------|
| `ButtonBackgroundColor` | Default button background |
| `ButtonForegroundColor` | Default button text |
| `ButtonBorderColor` | Default button border |
| `ButtonHoverBackgroundColor` | Default button hover |
| `ButtonPressedBackgroundColor` | Default button pressed |

#### Primary Button Colors

| Key | Description |
|-----|-------------|
| `PrimaryButtonBackgroundColor` | Primary button background |
| `PrimaryButtonForegroundColor` | Primary button text |
| `PrimaryButtonHoverBackgroundColor` | Primary button hover |
| `PrimaryButtonPressedBackgroundColor` | Primary button pressed |

#### Danger Button Colors

| Key | Description |
|-----|-------------|
| `DangerButtonBackgroundColor` | Danger button background |
| `DangerButtonForegroundColor` | Danger button text |
| `DangerButtonHoverBackgroundColor` | Danger button hover |
| `DangerButtonPressedBackgroundColor` | Danger button pressed |

#### Icon Colors

| Key | Description |
|-----|-------------|
| `InfoIconColor` | Information icon |
| `WarningIconColor` | Warning icon |
| `ErrorIconColor` | Error icon |
| `SuccessIconColor` | Success icon |
| `QuestionIconColor` | Question icon |

#### Progress Bar Colors

| Key | Description |
|-----|-------------|
| `ProgressBarBackgroundColor` | Progress bar track |
| `ProgressBarForegroundColor` | Progress bar fill |

#### Toast Colors

| Key | Description |
|-----|-------------|
| `ToastBackgroundColor` | Toast background |
| `ToastForegroundColor` | Toast primary text |
| `ToastSecondaryForegroundColor` | Toast secondary text |
| `ToastBorderColor` | Toast border |

---

## Creating a Custom Theme

Create a complete custom theme by defining all color keys:

### CustomTheme.xaml

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/ThemeResources.xaml"/>
    </ResourceDictionary.MergedDictionaries>

    <!-- Your custom colors -->
    <Color x:Key="DialogBackgroundColor">#1A1A2E</Color>
    <Color x:Key="DialogForegroundColor">#EAEAEA</Color>
    <Color x:Key="DialogSecondaryForegroundColor">#A0A0A0</Color>
    <Color x:Key="DialogBorderColor">#2A2A4E</Color>
    
    <Color x:Key="ButtonBackgroundColor">#2A2A4E</Color>
    <Color x:Key="ButtonForegroundColor">#EAEAEA</Color>
    <Color x:Key="ButtonBorderColor">#3A3A6E</Color>
    <Color x:Key="ButtonHoverBackgroundColor">#3A3A6E</Color>
    <Color x:Key="ButtonPressedBackgroundColor">#1A1A2E</Color>
    
    <Color x:Key="PrimaryButtonBackgroundColor">#E94560</Color>
    <Color x:Key="PrimaryButtonForegroundColor">#FFFFFF</Color>
    <Color x:Key="PrimaryButtonHoverBackgroundColor">#D63850</Color>
    <Color x:Key="PrimaryButtonPressedBackgroundColor">#C02040</Color>
    
    <!-- ... more colors ... -->
</ResourceDictionary>
```

### Use Custom Theme

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Themes/CustomTheme.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

---

## Theme Synchronization

### Sync with System Theme

Detect and sync with Windows system theme:

```csharp
using Microsoft.Win32;

public class ThemeManager
{
    private readonly IMessageBoxThemeService _themeService;

    public ThemeManager(IMessageBoxThemeService themeService)
    {
        _themeService = themeService;
        
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
        _themeService.SetTheme(isDark ? MessageBoxTheme.Dark : MessageBoxTheme.Light);
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
    private readonly IMessageBoxThemeService _themeService;

    public SettingsViewModel(IMessageBoxThemeService themeService)
    {
        _themeService = themeService;
        
        // Load saved preference
        if (Enum.TryParse<MessageBoxTheme>(Settings.Default.Theme, out var theme))
        {
            _themeService.SetTheme(theme);
        }
        
        // Save on change
        _themeService.ThemeChanged += (_, e) =>
        {
            Settings.Default.Theme = e.NewTheme.ToString();
            Settings.Default.Save();
        };
    }
}
```

---

## Best Practices

1. **Merge ThemeResources.xaml first** - It contains required converters and base styles
2. **Override colors, not brushes** - The library creates brushes from colors dynamically
3. **Test both themes** - Ensure custom colors work in light and dark modes
4. **Use consistent colors** - Match your application's overall color scheme
5. **Consider accessibility** - Ensure sufficient contrast ratios
