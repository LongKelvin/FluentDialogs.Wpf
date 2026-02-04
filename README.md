# FluentDialogs.Wpf

A modern, injectable WPF dialog library for .NET 9 that replaces `System.Windows.MessageBox` with a fully themeable, MVVM-first approach aligned with Windows 11 Fluent Design.

## Features

- **Injectable via DI** - No static methods, fully testable
- **Async-first API** - All methods return `Task<MessageBoxResult>`
- **MVVM Compatible** - Works seamlessly with any MVVM framework
- **Windows 11 Fluent Design** - Modern rounded corners, shadows, and animations
- **Light/Dark Theme Support** - Runtime theme switching
- **Custom Buttons** - Define your own buttons with custom styles and commands
- **Exception Display** - Built-in expandable exception details for error dialogs
- **Custom Content** - Add checkboxes, hyperlinks, or any WPF content to dialogs

## Installation

```bash
dotnet add package FluentDialogs.Wpf
```

Or via Package Manager Console:

```powershell
Install-Package FluentDialogs.Wpf
```

## Quick Start

### 1. Register Services

```csharp
// In App.xaml.cs or your DI configuration
using FluentDialogs;

var services = new ServiceCollection();
services.AddFluentDialogs();

// Build the service provider
var serviceProvider = services.BuildServiceProvider();
```

### 2. Inject and Use

```csharp
public class MainViewModel
{
    private readonly IMessageBoxService _messageBoxService;

    public MainViewModel(IMessageBoxService messageBoxService)
    {
        _messageBoxService = messageBoxService;
    }

    public async Task ShowInfoAsync()
    {
        await _messageBoxService.InfoAsync(
            "Operation completed successfully.",
            "Success"
        );
    }

    public async Task<bool> ConfirmDeleteAsync()
    {
        var result = await _messageBoxService.ConfirmAsync(
            "Are you sure you want to delete this item?",
            "Confirm Delete"
        );

        return result == MessageBoxResult.Yes;
    }

    public async Task ShowErrorAsync(Exception ex)
    {
        await _messageBoxService.ErrorAsync(
            "An error occurred while processing your request.",
            ex
        );
    }
}
```

## API Reference

### IMessageBoxService

The main service interface for displaying dialogs.

| Method | Description |
|--------|-------------|
| `ShowAsync(MessageBoxOptions)` | Display a dialog with full customization |
| `InfoAsync(message, title?)` | Display an info dialog with OK button |
| `ConfirmAsync(message, title?)` | Display a confirmation dialog with Yes/No buttons |
| `ErrorAsync(message, exception?)` | Display an error dialog with optional exception details |

### MessageBoxOptions

Full customization options for dialogs.

```csharp
var options = new MessageBoxOptions
{
    Title = "Custom Dialog",
    Message = "This is a custom message.",
    Icon = MessageBoxIcon.Question,
    Buttons = MessageBoxButtons.YesNoCancel,
    Owner = Application.Current.MainWindow
};

var result = await _messageBoxService.ShowAsync(options);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Dialog title text |
| `Message` | `string` | Main message content |
| `Icon` | `MessageBoxIcon` | Icon to display (None, Info, Warning, Error, Success, Question) |
| `Buttons` | `MessageBoxButtons?` | Standard button configuration |
| `CustomButtons` | `IReadOnlyList<MessageBoxButtonDefinition>?` | Custom button definitions |
| `Content` | `object?` | Custom WPF content below the message |
| `Owner` | `Window?` | Owner window for modal behavior |
| `Exception` | `Exception?` | Exception to display with expandable details |

### Custom Buttons

Create dialogs with custom buttons and behaviors:

```csharp
var customButtons = new List<MessageBoxButtonDefinition>
{
    new()
    {
        Text = "Save",
        Result = MessageBoxResult.Yes,
        Style = ButtonStyle.Primary,
        IsDefault = true
    },
    new()
    {
        Text = "Don't Save",
        Result = MessageBoxResult.No,
        Style = ButtonStyle.Danger
    },
    new()
    {
        Text = "Cancel",
        Result = MessageBoxResult.Cancel,
        Style = ButtonStyle.Default,
        IsCancel = true
    }
};

var options = new MessageBoxOptions
{
    Title = "Unsaved Changes",
    Message = "Do you want to save changes before closing?",
    Icon = MessageBoxIcon.Warning,
    CustomButtons = customButtons
};

var result = await _messageBoxService.ShowAsync(options);
```

#### Button Styles

| Style | Description |
|-------|-------------|
| `Default` | Standard button appearance |
| `Primary` | Highlighted primary action (blue) |
| `Secondary` | Subtle secondary action |
| `Danger` | Destructive action (red) |

### Theme Support

Switch between light and dark themes at runtime:

```csharp
public class SettingsViewModel
{
    private readonly IMessageBoxThemeService _themeService;

    public SettingsViewModel(IMessageBoxThemeService themeService)
    {
        _themeService = themeService;
    }

    public void ToggleTheme()
    {
        var newTheme = _themeService.CurrentTheme == MessageBoxTheme.Light
            ? MessageBoxTheme.Dark
            : MessageBoxTheme.Light;

        _themeService.SetTheme(newTheme);
    }
}
```

### Custom Content

Add custom WPF content to dialogs:

```csharp
var checkBox = new CheckBox
{
    Content = "Don't show this message again",
    Margin = new Thickness(0, 8, 0, 0)
};

var options = new MessageBoxOptions
{
    Title = "Notice",
    Message = "This action will modify your settings.",
    Icon = MessageBoxIcon.Info,
    Buttons = MessageBoxButtons.OKCancel,
    Content = checkBox
};

var result = await _messageBoxService.ShowAsync(options);

if (result == MessageBoxResult.OK && checkBox.IsChecked == true)
{
    // User clicked OK and checked "Don't show again"
}
```

## Icon Types

| Icon | Usage |
|------|-------|
| `None` | No icon displayed |
| `Info` | Informational messages |
| `Warning` | Warning messages |
| `Error` | Error messages |
| `Success` | Success confirmations |
| `Question` | Questions requiring user decision |

## Button Configurations

| Configuration | Buttons |
|---------------|---------|
| `OK` | OK |
| `OKCancel` | OK, Cancel |
| `YesNo` | Yes, No |
| `YesNoCancel` | Yes, No, Cancel |
| `RetryCancel` | Retry, Cancel |
| `AbortRetryIgnore` | Abort, Retry, Ignore |

## Best Practices

1. **Always use async/await** - Never call `.Result` or `.Wait()` on dialog methods
2. **Inject services** - Use constructor injection for testability
3. **Set Owner** - When possible, set the Owner property for proper modal behavior
4. **Handle all results** - Always handle the returned `MessageBoxResult`

## Requirements

- .NET 9.0 or later
- Windows Desktop Runtime

## License

MIT License - see [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please read our contributing guidelines before submitting PRs.

## Screenshots

![Light Theme](docs/screenshot-light.png)
![Dark Theme](docs/screenshot-dark.png)
