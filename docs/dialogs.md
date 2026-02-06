# Dialog Types Guide

Complete guide to all dialog types in FluentDialogs.Wpf.

## Basic Dialogs

### Information Dialog

Display a simple informational message:

```csharp
await _messageBox.InfoAsync("Operation completed successfully!", "Success");
```

### Confirmation Dialog

Ask user for Yes/No confirmation:

```csharp
var result = await _messageBox.ConfirmAsync(
    "Are you sure you want to proceed?", 
    "Confirm Action");

if (result == MessageBoxResult.Yes)
{
    // User confirmed
}
```

### Warning Dialog

Display a warning message:

```csharp
await _messageBox.WarningAsync(
    "This action cannot be undone.", 
    "Warning");
```

### Error Dialog

Display an error with optional exception details:

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

The exception details are displayed in an expandable section.

---

## Input Dialogs

### Text Input

Collect text input from the user:

```csharp
var result = await _messageBox.InputAsync(
    message: "Please enter your name:",
    placeholder: "Name",
    defaultValue: "John Doe",
    title: "User Input");

if (result.Result == MessageBoxResult.OK)
{
    string name = result.InputText;
    // Use the input
}
```

### Password Input

Collect masked password input:

```csharp
var result = await _messageBox.InputAsync(
    message: "Enter your password:",
    placeholder: "Password",
    title: "Authentication",
    isPassword: true);

if (result.Result == MessageBoxResult.OK)
{
    string password = result.InputText;
    // Authenticate
}
```

---

## Selection Dialogs

### Simple Selection

Let user choose from a list:

```csharp
var options = new[] { "Option A", "Option B", "Option C" };

var result = await _messageBox.SelectAsync(
    message: "Choose an option:",
    items: options,
    defaultIndex: 0,
    title: "Selection");

if (result.Result == MessageBoxResult.OK)
{
    string selected = result.SelectedItem as string;
    int index = result.SelectedIndex;
}
```

### Complex Object Selection

Select from complex objects with display member:

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
}

var products = new[]
{
    new Product { Id = 1, Name = "Widget" },
    new Product { Id = 2, Name = "Gadget" },
    new Product { Id = 3, Name = "Gizmo" }
};

var result = await _messageBox.SelectAsync(
    message: "Select a product:",
    items: products,
    displayMemberPath: "Name",
    title: "Products");

if (result.Result == MessageBoxResult.OK)
{
    var product = result.SelectedItem as Product;
}
```

---

## Checkbox Dialogs

### Confirmation with Checkbox

Add a "Don't show again" checkbox:

```csharp
var result = await _messageBox.ConfirmWithCheckboxAsync(
    message: "Would you like to save changes?",
    checkboxText: "Don't ask me again",
    title: "Save Changes");

if (result.Result == MessageBoxResult.Yes)
{
    // Save changes
}

if (result.IsChecked)
{
    // Remember the preference
    Settings.DontAskAgain = true;
}
```

---

## License/Disclaimer Dialogs

Display long-form text with scroll requirement:

```csharp
var result = await _messageBox.LicenseAsync(
    title: "End User License Agreement",
    message: "Please read and accept the license agreement:",
    detailedText: licenseText,
    requireScrollToBottom: true);

if (result.Result == MessageBoxResult.OK)
{
    // User accepted the license
}
```

The Accept button is disabled until the user scrolls to the bottom of the text.

---

## Timeout Dialogs

Auto-closing dialogs for time-sensitive notifications:

```csharp
var result = await _messageBox.TimeoutAsync(
    message: "Your session will expire in 10 seconds. Click OK to extend.",
    timeoutSeconds: 10,
    timeoutResult: MessageBoxResult.Cancel,
    title: "Session Expiring");

if (result.TimedOut)
{
    // Dialog auto-closed, log out user
    LogOut();
}
else if (result.Result == MessageBoxResult.OK)
{
    // User clicked OK, extend session
    ExtendSession();
}
```

---

## Custom Button Dialogs

Create dialogs with custom buttons:

```csharp
var buttons = new List<MessageBoxButtonDefinition>
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
        IsCancel = true
    }
};

var options = new MessageBoxOptions
{
    Title = "Unsaved Changes",
    Message = "Do you want to save your changes before closing?",
    Icon = MessageBoxIcon.Warning,
    CustomButtons = buttons
};

var result = await _messageBox.ShowAsync(options);

switch (result)
{
    case MessageBoxResult.Yes:
        await SaveAsync();
        break;
    case MessageBoxResult.No:
        // Discard changes
        break;
    case MessageBoxResult.Cancel:
        // Abort close
        break;
}
```

### Button with Command

Execute a command when button is clicked:

```csharp
var buttons = new List<MessageBoxButtonDefinition>
{
    new()
    {
        Text = "Delete",
        Result = MessageBoxResult.Yes,
        Style = ButtonStyle.Danger,
        Command = DeleteCommand,
        CommandParameter = itemToDelete
    },
    new()
    {
        Text = "Cancel",
        Result = MessageBoxResult.Cancel,
        IsCancel = true
    }
};
```

---

## Custom Content Dialogs

Inject any WPF content:

```csharp
// Simple checkbox
var checkbox = new CheckBox { Content = "I agree to the terms" };

var options = new MessageBoxOptions
{
    Title = "Terms of Service",
    Message = "Please review and accept our terms:",
    Content = checkbox,
    Buttons = MessageBoxButtons.OKCancel
};

var result = await _messageBox.ShowAsync(options);

if (result == MessageBoxResult.OK && checkbox.IsChecked == true)
{
    // User agreed
}
```

### Complex Custom Content

```csharp
var panel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
panel.Children.Add(new TextBlock { Text = "Additional options:" });
panel.Children.Add(new CheckBox { Content = "Enable feature A", Margin = new Thickness(0, 5, 0, 0) });
panel.Children.Add(new CheckBox { Content = "Enable feature B", Margin = new Thickness(0, 5, 0, 0) });

var options = new MessageBoxOptions
{
    Title = "Settings",
    Message = "Configure your preferences:",
    Content = panel,
    Buttons = MessageBoxButtons.OKCancel
};
```

---

## Hyperlink Dialogs

Add clickable links:

```csharp
var options = new MessageBoxOptions
{
    Title = "Update Available",
    Message = "A new version is available. Click the link below for more information.",
    Icon = MessageBoxIcon.Info,
    Buttons = MessageBoxButtons.OK,
    HyperlinkText = "View Release Notes",
    HyperlinkUrl = "https://github.com/example/releases"
};

await _messageBox.ShowAsync(options);
```

---

## Dialog Sizing

Control dialog dimensions:

```csharp
var options = new MessageBoxOptions
{
    Title = "Large Content",
    Message = veryLongText,
    Buttons = MessageBoxButtons.OK,
    Width = 600,      // Fixed width
    Height = 400,     // Fixed height
    MinWidth = 400,   // Minimum width (default: 320)
    MaxHeight = 800   // Maximum height (default: 600)
};
```

---

## Owner Window

Set owner for proper modal behavior:

```csharp
var options = new MessageBoxOptions
{
    Title = "Confirm",
    Message = "Proceed?",
    Buttons = MessageBoxButtons.YesNo,
    Owner = Application.Current.MainWindow  // Or specific window
};

await _messageBox.ShowAsync(options);
```

When no owner is set, the dialog uses the currently active window.

---

## Error Handling

DialogResult when user closes without clicking a button:

```csharp
var result = await _messageBox.ConfirmAsync("Continue?");

// If user clicks X or presses Escape:
// result == MessageBoxResult.None

switch (result)
{
    case MessageBoxResult.Yes:
        // User confirmed
        break;
    case MessageBoxResult.No:
        // User declined
        break;
    case MessageBoxResult.None:
    case MessageBoxResult.Cancel:
        // Dialog was closed without a definitive answer
        break;
}
```
