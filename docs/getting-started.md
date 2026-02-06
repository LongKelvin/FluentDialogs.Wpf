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
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Register FluentDialogs services
        services.AddFluentDialogs();

        // Register your ViewModels
        services.AddTransient<MainViewModel>();

        ServiceProvider = services.BuildServiceProvider();
        base.OnStartup(e);
    }
}
```

This registers:
- `IMessageBoxService` - Modal dialogs (info, confirm, error, input, etc.)
- `IMessageBoxThemeService` - Theme management (light/dark)
- `IToastService` - Non-blocking toast notifications

### 2. Merge Theme Resources

In your `App.xaml`, add the theme resources:

```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/ThemeResources.xaml"/>
                <ResourceDictionary Source="pack://application:,,,/FluentDialogs.Wpf;component/Themes/FluentLight.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

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

## Next Steps

- [API Reference](api-reference.md) - Complete API documentation
- [Dialogs Guide](dialogs.md) - All dialog types and options
- [Progress Dialogs](progress.md) - Long-running operations
- [Toast Notifications](toasts.md) - Non-blocking notifications
- [Theming](theming.md) - Customizing appearance
- [Fluent Builder](builder.md) - Fluent API for dialogs
