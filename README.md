# FluentDialogs.Wpf

Injectable WPF dialog library for .NET 9. Replaces `System.Windows.MessageBox` with async, MVVM-compatible dialogs styled for Windows 11 Fluent Design.

## What This Library Is

A dependency-injectable message box replacement for WPF applications. Provides async dialog methods, custom buttons, theming (light/dark), and extended dialog types (input, selection, license, timeout). All dialogs are modal, respect owner windows, and return strongly-typed results.

## Installation

```bash
dotnet add package FluentDialogs.Wpf
```

**Requirements:** .NET 9.0+, Windows Desktop Runtime

## Basic Setup

### DI Registration

```csharp
using FluentDialogs;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddFluentDialogs();
var provider = services.BuildServiceProvider();
```

This registers:
- `IMessageBoxService` - Main dialog service
- `IMessageBoxThemeService` - Theme switching

For manual theme management:
```csharp
services.AddFluentDialogsWithoutTheme();
```

### Required Resources

Merge theme resources in `App.xaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/ThemeResources.xaml"/>
            <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentLight.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Basic Usage

Inject `IMessageBoxService` into your ViewModel:

```csharp
public class MainViewModel
{
    private readonly IMessageBoxService _messageBox;

    public MainViewModel(IMessageBoxService messageBox)
    {
        _messageBox = messageBox;
    }

    public async Task ShowInfoAsync()
    {
        await _messageBox.InfoAsync("Operation completed.", "Info");
    }

    public async Task<bool> ConfirmAsync()
    {
        var result = await _messageBox.ConfirmAsync("Delete this item?", "Confirm");
        return result == MessageBoxResult.Yes;
    }

    public async Task ShowErrorAsync(Exception ex)
    {
        await _messageBox.ErrorAsync("Operation failed.", ex);
    }
}
```

### Available Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ShowAsync(options)` | `MessageBoxResult` | Full customization via options |
| `ShowExtendedAsync(options)` | `DialogResult` | Returns extended data (checkbox, input, etc.) |
| `InfoAsync(message, title?)` | `MessageBoxResult` | Info dialog, OK button |
| `ConfirmAsync(message, title?)` | `MessageBoxResult` | Yes/No dialog |
| `WarningAsync(message, title?)` | `MessageBoxResult` | Warning dialog, OK button |
| `ErrorAsync(message, exception?)` | `MessageBoxResult` | Error with expandable exception |
| `InputAsync(...)` | `DialogResult` | Text input dialog |
| `SelectAsync<T>(...)` | `DialogResult` | Selection list dialog |
| `ConfirmWithCheckboxAsync(...)` | `DialogResult` | Confirm with checkbox |
| `LicenseAsync(...)` | `DialogResult` | Scrollable license text |
| `TimeoutAsync(...)` | `DialogResult` | Auto-closing dialog |

## Custom Buttons

Define buttons with custom text, styles, and behavior:

```csharp
var buttons = new List<MessageBoxButtonDefinition>
{
    new() { Text = "Save", Result = MessageBoxResult.Yes, Style = ButtonStyle.Primary, IsDefault = true },
    new() { Text = "Discard", Result = MessageBoxResult.No, Style = ButtonStyle.Danger },
    new() { Text = "Cancel", Result = MessageBoxResult.Cancel, IsCancel = true }
};

var options = new MessageBoxOptions
{
    Title = "Unsaved Changes",
    Message = "Save before closing?",
    Icon = MessageBoxIcon.Warning,
    CustomButtons = buttons
};

var result = await _messageBox.ShowAsync(options);
```

### Button Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Button label |
| `Result` | `MessageBoxResult` | Return value when clicked |
| `Style` | `ButtonStyle` | Visual style (Default, Primary, Secondary, Danger) |
| `IsDefault` | `bool` | Activated on Enter key |
| `IsCancel` | `bool` | Activated on Escape key |
| `Command` | `ICommand?` | Execute before dialog closes |

**Constraint:** Use either `Buttons` or `CustomButtons`, not both.

## Custom Content

Inject any WPF content below the message:

```csharp
var options = new MessageBoxOptions
{
    Title = "Settings",
    Message = "Apply changes?",
    Buttons = MessageBoxButtons.OKCancel,
    Content = new CheckBox { Content = "Remember choice" }
};

var result = await _messageBox.ShowAsync(options);
```

The `Content` property accepts any `object` renderable by WPF ContentPresenter.

### Built-in Native Features

Use options properties instead of custom content for common scenarios:

```csharp
// Checkbox
var result = await _messageBox.ConfirmWithCheckboxAsync(
    "Continue?", "Do not ask again", "Confirm");
// result.IsChecked contains checkbox state

// Input
var result = await _messageBox.InputAsync(
    "Enter name:", "Name", defaultValue: "User");
// result.InputText contains input

// Password
var result = await _messageBox.InputAsync(
    "Enter password:", "Password", isPassword: true);

// Selection
var result = await _messageBox.SelectAsync(
    "Choose option:", new[] { "A", "B", "C" });
// result.SelectedItem, result.SelectedIndex

// Hyperlink (via options)
var options = new MessageBoxOptions
{
    Message = "Click link for help.",
    HyperlinkText = "Documentation",
    HyperlinkUrl = "https://example.com"
};
```

## Dialog Sizing

Dialogs auto-size by default. Override with explicit dimensions:

```csharp
var options = new MessageBoxOptions
{
    Title = "Large Content",
    Message = longText,
    Width = 600,
    Height = 400,
    MinWidth = 400,
    MaxHeight = 800
};
```

| Property | Default | Description |
|----------|---------|-------------|
| `Width` | auto | Fixed width in pixels |
| `Height` | auto | Fixed height in pixels |
| `MinWidth` | 320 | Minimum width |
| `MinHeight` | 150 | Minimum height |
| `MaxWidth` | 800 | Maximum width |
| `MaxHeight` | 600 | Maximum height |

When `Width` or `Height` is set, auto-sizing is disabled for that dimension.

## Theming

### Light/Dark Switching

```csharp
public class SettingsViewModel
{
    private readonly IMessageBoxThemeService _theme;

    public SettingsViewModel(IMessageBoxThemeService theme) => _theme = theme;

    public void SetDarkMode(bool dark)
    {
        _theme.SetTheme(dark ? MessageBoxTheme.Dark : MessageBoxTheme.Light);
    }
}
```

### Custom Title Bar Color

```csharp
using System.Windows.Media;

var options = new MessageBoxOptions
{
    Title = "Custom Header",
    Message = "Title bar uses custom color.",
    TitleBarColor = Colors.DarkBlue
};
```

Title text color adjusts automatically for contrast.

### Resource Overrides

Override colors by redefining keys after merging base resources:

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentLight.xaml"/>
</ResourceDictionary.MergedDictionaries>

<!-- Override primary button color -->
<Color x:Key="PrimaryButtonBackgroundColor">#00A86B</Color>
```

Key color resources: `DialogBackgroundColor`, `DialogForegroundColor`, `PrimaryButtonBackgroundColor`, `DangerButtonBackgroundColor`, etc.

## Common Scenarios

### Error with Exception Details

```csharp
try
{
    await SomeOperationAsync();
}
catch (Exception ex)
{
    await _messageBox.ErrorAsync("Operation failed.", ex);
}
```

Shows expandable stack trace.

### License Agreement

```csharp
var result = await _messageBox.LicenseAsync(
    title: "License Agreement",
    message: "Please read and accept:",
    detailedText: licenseText,
    requireScrollToBottom: true);

if (result.Result == MessageBoxResult.OK)
{
    // User accepted
}
```

Accept button is disabled until user scrolls to bottom.

### Auto-Closing Dialog

```csharp
var result = await _messageBox.TimeoutAsync(
    message: "Session expires in 10 seconds.",
    timeoutSeconds: 10,
    timeoutResult: MessageBoxResult.Cancel);

if (result.TimedOut)
{
    // Dialog auto-closed
}
```

## DialogResult Properties

Extended dialogs return `DialogResult` with additional data:

| Property | Type | Description |
|----------|------|-------------|
| `Result` | `MessageBoxResult` | Button clicked |
| `IsChecked` | `bool` | Checkbox state |
| `InputText` | `string?` | Text input value |
| `SelectedItem` | `object?` | Selected list item |
| `SelectedIndex` | `int` | Selected index (-1 if none) |
| `TimedOut` | `bool` | True if closed by timeout |

`DialogResult` implicitly converts to `MessageBoxResult`.

## Rules and Limitations

### Intended For
- Modal confirmation dialogs
- User notifications (info, warning, error)
- Simple input collection
- License/disclaimer acceptance

### Not Intended For
- Complex forms (use dedicated windows)
- Non-blocking notifications (use toast/snackbar)
- File/folder selection (use system dialogs)
- Long-running progress (use progress windows)

### Constraints
- All methods are async; do not block with `.Result` or `.Wait()`
- Dialogs are always modal
- Cannot specify both `Buttons` and `CustomButtons`
- At most one `IsDefault` and one `IsCancel` button allowed
- `Owner` should be set for proper modal behavior when possible

## License

MIT
