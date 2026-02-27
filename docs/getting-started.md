# Getting Started with FluentDialogs.Wpf

This guide will help you integrate FluentDialogs.Wpf into your WPF application.

## Prerequisites

- .NET 9.0 or later
- Windows Desktop Runtime
- A WPF application project

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package FluentDialogs.Wpf
```

Or via Package Manager Console:

```powershell
Install-Package FluentDialogs.Wpf
```

## Quick Setup

### 1. Register Services

In your `App.xaml.cs` or wherever you configure dependency injection:

```csharp
using FluentDialogs;
using FluentDialogs.Enums;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Register FluentDialogs services
        services.AddFluentDialogs();

        // Or with configuration
        services.AddFluentDialogs(options =>
        {
            options.DefaultPreset = MessageBoxTheme.Light; // or .Dark
            // options.AccentColor = Colors.Purple;        // optional brand color
        });

        // Register your ViewModels
        services.AddTransient<MainViewModel>();

        ServiceProvider = services.BuildServiceProvider();
        base.OnStartup(e);
    }
}
```

This registers:

| Service | Description |
|---------|-------------|
| `IMessageBoxService` | Modal dialogs (info, confirm, error, input, dropdown, etc.) |
| `IFluentDialogThemeService` | v2 theme management — presets, tokens, accent color |
| `IToastService` | Non-blocking toast notifications |
| `IMessageBoxThemeService` | Legacy v1 theme adapter (backward compatible) |

### 2. Merge Theme Resources

In your `App.xaml`, add the **single** theme entry point:

```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentDialogs.Theme.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

> **Note:** `FluentDialogs.Theme.xaml` is the single entry point that loads all foundation layers (primitives, semantics, brushes, typography, layout, infrastructure), control styles, the legacy compatibility layer, and the default Light preset — in the correct order.

### 3. Use in ViewModels

Inject `IMessageBoxService` into your ViewModel:

```csharp
using FluentDialogs.Abstractions;
using FluentDialogs.Models;

public class MainViewModel
{
    private readonly IMessageBoxService _messageBox;
    private readonly IToastService _toast;

    public MainViewModel(IMessageBoxService messageBox, IToastService toast)
    {
        _messageBox = messageBox;
        _toast = toast;
    }

    public async Task ShowInfoAsync()
    {
        await _messageBox.InfoAsync("Hello, World!", "Welcome");
    }

    public async Task DeleteItemAsync()
    {
        var result = await _messageBox.ConfirmAsync(
            "Are you sure you want to delete this item?",
            "Confirm Delete");

        if (result == MessageBoxResult.Yes)
        {
            // Perform delete
            _toast.ShowSuccess("Item deleted successfully!");
        }
    }
}
```

### 4. Switch Themes (Optional)

Inject `IFluentDialogThemeService` to switch between Light and Dark at runtime:

```csharp
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;

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

## Avoiding Namespace Conflicts

If you encounter conflicts with `System.Windows` types, use the Fluent-prefixed aliases:

```csharp
using FluentDialogs.Models;

// Use FluentMessageBoxOptions instead of MessageBoxOptions
var options = new FluentMessageBoxOptions
{
    Title = "Confirm",
    Message = "Proceed?",
    Buttons = MessageBoxButtons.YesNo
};

// Use FluentDialogResult instead of DialogResult
FluentDialogResult result = await _messageBox.ShowExtendedAsync(options);
```

Or use explicit using directives:

```csharp
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;
using DialogResult = FluentDialogs.Models.DialogResult;
```

## Migrating from v1

If you are upgrading from FluentDialogs v1:

1. **Replace App.xaml resource merges** — Remove the old `ThemeResources.xaml` + `FluentLight.xaml` entries and replace with the single `FluentDialogs.Theme.xaml`:

   ```xml
   <!-- OLD (v1) — remove these -->
   <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/ThemeResources.xaml"/>
   <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentLight.xaml"/>

   <!-- NEW (v2) — single entry point -->
   <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentDialogs.Theme.xaml"/>
   ```

2. **Update DI registration** — `services.AddFluentDialogs()` still works unchanged. Optionally add configuration:

   ```csharp
   services.AddFluentDialogs(options =>
   {
       options.DefaultPreset = MessageBoxTheme.Dark;
   });
   ```

3. **Theme switching** — `IMessageBoxThemeService` still works via a legacy adapter. For new code, prefer `IFluentDialogThemeService`.

4. **Custom color overrides** — v1 color keys (`DialogBackgroundColor`, `PrimaryButtonBackgroundColor`, etc.) still work via the backward-compatibility layer. For new customizations, use the v2 `FDSem*` semantic token keys. See the [Theming Guide](theming.md).

## Next Steps

- [API Reference](api-reference.md) — Complete API documentation
- [Dialogs Guide](dialogs.md) — All dialog types and options
- [Progress Dialogs](progress.md) — Long-running operations
- [Toast Notifications](toasts.md) — Non-blocking notifications
- [Theming Guide](theming.md) — Customizing appearance, tokens, presets
- [Fluent Builder](builder.md) — Fluent API for dialogs
