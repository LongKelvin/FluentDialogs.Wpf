# API Reference

Complete reference for all public types in FluentDialogs.Wpf.

## Services

### IMessageBoxService

Main service for displaying modal dialogs.

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ShowAsync(MessageBoxOptions)` | `Task<MessageBoxResult>` | Display dialog with full options |
| `ShowExtendedAsync(MessageBoxOptions)` | `Task<DialogResult>` | Display dialog with extended result |
| `InfoAsync(message, title?)` | `Task<MessageBoxResult>` | Information dialog with OK button |
| `ConfirmAsync(message, title?)` | `Task<MessageBoxResult>` | Confirmation with Yes/No buttons |
| `WarningAsync(message, title?)` | `Task<MessageBoxResult>` | Warning dialog with OK button |
| `ErrorAsync(message, exception?)` | `Task<MessageBoxResult>` | Error dialog with optional exception |
| `InputAsync(message, placeholder, defaultValue?, title?, isPassword?)` | `Task<DialogResult>` | Text input dialog |
| `SelectAsync<T>(message, items, displayMemberPath?, defaultIndex?, title?)` | `Task<DialogResult>` | Selection list dialog |
| `ConfirmWithCheckboxAsync(message, checkboxText, title?)` | `Task<DialogResult>` | Confirm with checkbox |
| `LicenseAsync(title, message, detailedText, requireScrollToBottom?)` | `Task<DialogResult>` | License/disclaimer dialog |
| `TimeoutAsync(message, timeoutSeconds, timeoutResult?, title?)` | `Task<DialogResult>` | Auto-closing dialog |
| `ShowProgressAsync(ProgressOptions)` | `Task<IProgressController>` | Progress dialog |
| `RunWithProgressAsync<T>(operation, options)` | `Task<T?>` | Execute operation with progress |
| `RunWithProgressAsync(operation, options)` | `Task` | Execute operation with progress |

### IToastService

Service for displaying non-modal toast notifications.

#### Methods

| Method | Description |
|--------|-------------|
| `Show(ToastOptions)` | Display toast with full options |
| `ShowInfo(message, title?, duration?)` | Information toast |
| `ShowSuccess(message, title?, duration?)` | Success toast |
| `ShowWarning(message, title?, duration?)` | Warning toast |
| `ShowError(message, title?, duration?)` | Error toast |
| `CloseAll()` | Close all displayed toasts |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `DefaultPosition` | `ToastPosition` | Default screen position |
| `MaxToasts` | `int` | Maximum simultaneous toasts |

### IMessageBoxThemeService

Service for managing dialog themes.

#### Methods

| Method | Description |
|--------|-------------|
| `SetTheme(MessageBoxTheme)` | Set the active theme |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `CurrentTheme` | `MessageBoxTheme` | Currently active theme |

#### Events

| Event | Description |
|-------|-------------|
| `ThemeChanged` | Raised when theme changes |

### IProgressController

Controller for active progress dialogs.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsCancellationRequested` | `bool` | Whether user requested cancellation |
| `CancellationToken` | `CancellationToken` | Token for cooperative cancellation |

#### Methods

| Method | Description |
|--------|-------------|
| `SetProgress(double)` | Set progress value (0-100) |
| `SetMessage(string)` | Update displayed message |
| `SetIndeterminate(bool)` | Toggle indeterminate mode |
| `CloseAsync()` | Close the progress dialog |

---

## Models

### MessageBoxOptions / FluentMessageBoxOptions

Configuration for message box dialogs.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | `""` | Dialog title |
| `Message` | `string` | `""` | Main message |
| `Icon` | `MessageBoxIcon` | `None` | Dialog icon |
| `Buttons` | `MessageBoxButtons?` | `null` | Standard buttons |
| `CustomButtons` | `IReadOnlyList<MessageBoxButtonDefinition>?` | `null` | Custom buttons |
| `Content` | `object?` | `null` | Custom WPF content |
| `Owner` | `Window?` | `null` | Owner window |
| `IconContent` | `Geometry?` | `null` | Custom icon geometry |
| `Exception` | `Exception?` | `null` | Exception to display |
| `Width` | `double?` | `null` | Fixed width |
| `Height` | `double?` | `null` | Fixed height |
| `MinWidth` | `double?` | `320` | Minimum width |
| `MinHeight` | `double?` | `150` | Minimum height |
| `MaxWidth` | `double?` | `800` | Maximum width |
| `MaxHeight` | `double?` | `600` | Maximum height |
| `TitleBarColor` | `Color?` | `null` | Custom title bar color |
| `CheckboxText` | `string?` | `null` | Checkbox label |
| `CheckboxChecked` | `bool` | `false` | Initial checkbox state |
| `HyperlinkText` | `string?` | `null` | Hyperlink text |
| `HyperlinkUrl` | `string?` | `null` | Hyperlink URL |
| `InputPlaceholder` | `string?` | `null` | Input placeholder |
| `InputDefaultValue` | `string?` | `null` | Initial input value |
| `InputIsPassword` | `bool` | `false` | Mask input as password |
| `SelectionItems` | `IReadOnlyList<object>?` | `null` | Selection list items |
| `SelectionDisplayMemberPath` | `string?` | `null` | Display property path |
| `SelectionDefaultIndex` | `int` | `-1` | Initial selected index |
| `TimeoutSeconds` | `int` | `0` | Auto-close timeout |
| `TimeoutResult` | `MessageBoxResult` | `Cancel` | Result on timeout |
| `DetailedText` | `string?` | `null` | Long-form content |
| `RequireScrollToBottom` | `bool` | `false` | Require scroll for accept |

### DialogResult / FluentDialogResult

Extended result from dialog operations.

| Property | Type | Description |
|----------|------|-------------|
| `Result` | `MessageBoxResult` | Button clicked |
| `IsChecked` | `bool` | Checkbox state |
| `InputText` | `string?` | Input field value |
| `SelectedItem` | `object?` | Selected list item |
| `SelectedIndex` | `int` | Selected index (-1 if none) |
| `TimedOut` | `bool` | Whether dialog timed out |

### MessageBoxButtonDefinition

Custom button definition.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `""` | Button label |
| `Result` | `MessageBoxResult` | `None` | Return value |
| `IsDefault` | `bool` | `false` | Activated on Enter |
| `IsCancel` | `bool` | `false` | Activated on Escape |
| `Command` | `ICommand?` | `null` | Command to execute |
| `CommandParameter` | `object?` | `null` | Command parameter |
| `Style` | `ButtonStyle` | `Default` | Visual style |

### ProgressOptions / FluentProgressOptions

Configuration for progress dialogs.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | `"Progress"` | Dialog title |
| `Message` | `string` | `""` | Status message |
| `IsIndeterminate` | `bool` | `true` | Unknown duration |
| `IsCancellable` | `bool` | `true` | Show cancel button |
| `Owner` | `Window?` | `null` | Owner window |
| `ShowPercentage` | `bool` | `true` | Show percentage text |
| `InitialProgress` | `double` | `0` | Initial value |
| `Width` | `double?` | `null` | Fixed width |

### ToastOptions / FluentToastOptions

Configuration for toast notifications.

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

### ThemeChangedEventArgs

Event data for theme changes.

| Property | Type | Description |
|----------|------|-------------|
| `OldTheme` | `MessageBoxTheme` | Previous theme |
| `NewTheme` | `MessageBoxTheme` | New theme |

---

## Enums

### MessageBoxIcon

Dialog icons.

| Value | Description |
|-------|-------------|
| `None` | No icon |
| `Info` | Information (blue i) |
| `Warning` | Warning (yellow !) |
| `Error` | Error (red x) |
| `Question` | Question (blue ?) |
| `Success` | Success (green ✓) |

### MessageBoxButtons

Standard button configurations.

| Value | Buttons |
|-------|---------|
| `OK` | OK |
| `OKCancel` | OK, Cancel |
| `YesNo` | Yes, No |
| `YesNoCancel` | Yes, No, Cancel |
| `RetryCancel` | Retry, Cancel |
| `AbortRetryIgnore` | Abort, Retry, Ignore |

### ButtonStyle

Button visual styles.

| Value | Description |
|-------|-------------|
| `Default` | Standard button |
| `Primary` | Highlighted/accent button |
| `Secondary` | Less emphasis |
| `Danger` | Destructive action (red) |

### MessageBoxTheme

Available themes.

| Value | Description |
|-------|-------------|
| `Light` | Light theme (Windows 11 style) |
| `Dark` | Dark theme |

### ToastType

Toast notification types.

| Value | Description |
|-------|-------------|
| `Info` | Informational (blue) |
| `Success` | Success (green) |
| `Warning` | Warning (orange) |
| `Error` | Error (red) |

### ToastPosition

Toast screen positions.

| Value | Description |
|-------|-------------|
| `TopRight` | Top-right corner |
| `TopLeft` | Top-left corner |
| `BottomRight` | Bottom-right corner |
| `BottomLeft` | Bottom-left corner |
| `TopCenter` | Top-center |
| `BottomCenter` | Bottom-center |

---

## Builder

### MessageBoxBuilder

Fluent API for building dialogs.

```csharp
await MessageBoxBuilder.Create(_messageBoxService)
    .WithTitle("Confirm")
    .WithMessage("Delete item?")
    .WithIcon(MessageBoxIcon.Warning)
    .WithButtons(MessageBoxButtons.YesNo)
    .OnYes(() => DeleteItem())
    .OnNo(() => Console.WriteLine("Cancelled"))
    .ShowAsync();
```

#### Builder Methods

| Method | Description |
|--------|-------------|
| `Create(IMessageBoxService)` | Create new builder |
| `WithTitle(string)` | Set title |
| `WithMessage(string)` | Set message |
| `WithIcon(MessageBoxIcon)` | Set icon |
| `WithButtons(MessageBoxButtons)` | Set standard buttons |
| `WithCustomButtons(...)` | Set custom buttons |
| `WithOwner(Window)` | Set owner window |
| `WithContent(object)` | Set custom content |
| `WithCheckbox(text, checked?)` | Add checkbox |
| `WithInput(placeholder, default?, password?)` | Add input |
| `WithTitleBarColor(Color)` | Set title bar color |
| `WithSize(width?, height?)` | Set dimensions |
| `OnYes(Action)` | Yes button callback |
| `OnNo(Action)` | No button callback |
| `OnOk(Action)` | OK button callback |
| `OnCancel(Action)` | Cancel button callback |
| `OnResult(Action<DialogResult>)` | Result callback |
| `ShowAsync()` | Display and return result |
| `BuildOptions()` | Build options without showing |

#### Extension Methods

```csharp
_messageBoxService.Confirm("Delete?").OnYes(() => Delete()).ShowAsync();
_messageBoxService.Info("Done!").ShowAsync();
_messageBoxService.Warning("Careful!").ShowAsync();
_messageBoxService.Error("Failed!").ShowAsync();
_messageBoxService.Input("Name:", "Enter name").ShowAsync();
```
