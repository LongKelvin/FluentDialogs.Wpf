# Toast Notifications

Guide to displaying non-blocking toast notifications.

## Overview

Toast notifications are lightweight, non-modal messages that appear briefly on screen. They're ideal for:

- Status updates
- Operation confirmations
- Non-critical information
- Feedback that doesn't require user action

Unlike modal dialogs, toasts don't block the user interface.

---

## Basic Usage

### Quick Methods

```csharp
// Inject IToastService
public class MyViewModel
{
    private readonly IToastService _toast;

    public MyViewModel(IToastService toast)
    {
        _toast = toast;
    }

    public void ShowNotifications()
    {
        // Information
        _toast.ShowInfo("Data loaded successfully.");

        // Success
        _toast.ShowSuccess("File saved!");

        // Warning
        _toast.ShowWarning("Connection is slow.");

        // Error
        _toast.ShowError("Failed to connect to server.");
    }
}
```

### With Title

```csharp
_toast.ShowSuccess("Your changes have been saved.", "Saved");
_toast.ShowError("Unable to reach the server.", "Connection Error");
```

### Custom Duration

```csharp
// Show for 10 seconds
_toast.ShowInfo("Important message", duration: TimeSpan.FromSeconds(10));

// Show indefinitely (must be clicked to close)
_toast.ShowWarning("Action required", duration: TimeSpan.Zero);
```

---

## Toast Options

For full control, use `ToastOptions`:

```csharp
_toast.Show(new ToastOptions
{
    Message = "Custom toast notification",
    Title = "Notice",
    Type = ToastType.Info,
    Duration = TimeSpan.FromSeconds(5),
    Position = ToastPosition.BottomRight,
    IsClickToClose = true
});
```

### ToastOptions Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Message` | `string` | `""` | Toast message |
| `Title` | `string?` | `null` | Optional title |
| `Type` | `ToastType` | `Info` | Visual style |
| `Duration` | `TimeSpan` | `4s` | Display duration |
| `IsClickToClose` | `bool` | `true` | Close on click |
| `Position` | `ToastPosition` | `TopRight` | Screen position |
| `OnClick` | `Action?` | `null` | Click callback |
| `OnClose` | `Action?` | `null` | Close callback |

---

## Toast Types

### ToastType Enum

| Value | Color | Icon | Use Case |
|-------|-------|------|----------|
| `Info` | Blue | ℹ | General information |
| `Success` | Green | ✓ | Successful operations |
| `Warning` | Orange | ⚠ | Potential issues |
| `Error` | Red | ✕ | Errors and failures |

```csharp
_toast.Show(new ToastOptions
{
    Message = "Operation completed",
    Type = ToastType.Success
});
```

---

## Toast Positions

### ToastPosition Enum

| Value | Description |
|-------|-------------|
| `TopRight` | Top-right corner (default) |
| `TopLeft` | Top-left corner |
| `TopCenter` | Top-center |
| `BottomRight` | Bottom-right corner |
| `BottomLeft` | Bottom-left corner |
| `BottomCenter` | Bottom-center |

### Set Default Position

```csharp
public class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // ...
        var toastService = ServiceProvider.GetRequiredService<IToastService>();
        toastService.DefaultPosition = ToastPosition.BottomRight;
    }
}
```

### Per-Toast Position

```csharp
_toast.Show(new ToastOptions
{
    Message = "Bottom notification",
    Position = ToastPosition.BottomCenter
});
```

---

## Callbacks

### OnClick Callback

Execute action when toast is clicked:

```csharp
_toast.Show(new ToastOptions
{
    Message = "Click to view details",
    Title = "New Message",
    Type = ToastType.Info,
    OnClick = () => OpenMessageDetails()
});
```

### OnClose Callback

Execute action when toast closes (by click, timeout, or programmatically):

```csharp
_toast.Show(new ToastOptions
{
    Message = "Processing complete",
    Type = ToastType.Success,
    OnClose = () => RefreshData()
});
```

---

## Managing Toasts

### Maximum Toasts

Control how many toasts display simultaneously:

```csharp
_toast.MaxToasts = 3; // Oldest toasts removed when limit exceeded
```

### Close All Toasts

```csharp
_toast.CloseAll();
```

---

## Common Patterns

### After Save Operation

```csharp
public async Task SaveAsync()
{
    try
    {
        await _repository.SaveAsync(data);
        _toast.ShowSuccess("Changes saved successfully!");
    }
    catch (Exception ex)
    {
        _toast.ShowError($"Save failed: {ex.Message}");
    }
}
```

### Background Task Completion

```csharp
public void StartBackgroundTask()
{
    Task.Run(async () =>
    {
        await LongRunningOperation();
        
        // Toast service handles UI thread marshaling
        _toast.ShowSuccess("Background task completed!");
    });
}
```

### Undo Action

```csharp
public void DeleteItem(Item item)
{
    _items.Remove(item);
    
    _toast.Show(new ToastOptions
    {
        Message = "Item deleted",
        Type = ToastType.Info,
        Duration = TimeSpan.FromSeconds(5),
        OnClick = () =>
        {
            _items.Add(item);
            _toast.ShowInfo("Deletion undone");
        }
    });
}
```

### Connection Status

```csharp
public void OnConnectionLost()
{
    // Persistent warning until connection restored
    _toast.Show(new ToastOptions
    {
        Message = "Connection lost. Retrying...",
        Type = ToastType.Warning,
        Duration = TimeSpan.Zero,  // Won't auto-close
        IsClickToClose = false     // Can't dismiss
    });
}

public void OnConnectionRestored()
{
    _toast.CloseAll();
    _toast.ShowSuccess("Connection restored!");
}
```

---

## Thread Safety

The `IToastService` handles UI thread marshaling automatically. You can call toast methods from any thread:

```csharp
await Task.Run(() =>
{
    // Called from background thread - works correctly
    _toast.ShowInfo("Background operation complete");
});
```

---

## Best Practices

1. **Keep messages short** - Toasts are for quick notifications
2. **Use appropriate types** - Match the type to the message importance
3. **Don't overuse** - Too many toasts become noise
4. **Set reasonable durations** - Important messages should display longer
5. **Consider position** - Bottom positions don't obscure content being edited
6. **Use callbacks wisely** - For undo/action toasts, not every notification
