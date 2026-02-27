# Fluent Builder API

The `MessageBoxBuilder` provides a chainable, fluent API for creating dialogs with callbacks.

## Overview

Instead of creating `MessageBoxOptions` directly, use the builder for a more readable, declarative syntax:

```csharp
// Traditional approach
var options = new MessageBoxOptions
{
    Title = "Confirm",
    Message = "Delete item?",
    Icon = MessageBoxIcon.Warning,
    Buttons = MessageBoxButtons.YesNo
};
var result = await _messageBox.ShowAsync(options);
if (result == MessageBoxResult.Yes)
{
    DeleteItem();
}

// Fluent builder approach
await _messageBox.Confirm("Delete item?")
    .OnYes(() => DeleteItem())
    .ShowAsync();
```

---

## Getting Started

### Create a Builder

```csharp
var builder = MessageBoxBuilder.Create(_messageBoxService);
```

### Configure and Show

```csharp
var result = await MessageBoxBuilder.Create(_messageBoxService)
    .WithTitle("Confirm Action")
    .WithMessage("Are you sure?")
    .WithIcon(MessageBoxIcon.Question)
    .WithButtons(MessageBoxButtons.YesNo)
    .ShowAsync();
```

---

## Extension Methods

For common dialog types, use the convenient extension methods on `IMessageBoxService`:

### Confirm

```csharp
await _messageBox.Confirm("Delete this item?")
    .OnYes(() => DeleteItem())
    .OnNo(() => Console.WriteLine("Cancelled"))
    .ShowAsync();

// With custom title
await _messageBox.Confirm("Proceed with operation?", "Confirm Operation")
    .OnYes(() => Proceed())
    .ShowAsync();
```

### Info

```csharp
await _messageBox.Info("Operation completed successfully!")
    .ShowAsync();

// With callback
await _messageBox.Info("File saved.")
    .OnOk(() => CloseWindow())
    .ShowAsync();
```

### Warning

```csharp
await _messageBox.Warning("This action cannot be undone.")
    .ShowAsync();
```

### Error

```csharp
await _messageBox.Error("Failed to save file.")
    .ShowAsync();
```

### Input

```csharp
var result = await _messageBox.Input("Enter your name:", "Name")
    .OnOk(() => Console.WriteLine("Submitted"))
    .OnCancel(() => Console.WriteLine("Cancelled"))
    .ShowAsync();

if (result.Result == MessageBoxResult.OK)
{
    string name = result.InputText;
}
```

### Dropdown

```csharp
var colors = new[] { "Red", "Green", "Blue", "Purple" };

var result = await _messageBox.Dropdown("Choose a color:", colors, defaultIndex: 1)
    .OnOk(() => Console.WriteLine("Selected!"))
    .OnCancel(() => Console.WriteLine("Cancelled"))
    .ShowAsync();

if (result.Result == MessageBoxResult.OK)
{
    string color = result.DropdownSelectedItem as string;
    int index = result.DropdownSelectedIndex;
}
```

---

## Builder Methods

### Content Configuration

```csharp
MessageBoxBuilder.Create(_messageBox)
    .WithTitle("Dialog Title")
    .WithMessage("Dialog message text")
    .WithIcon(MessageBoxIcon.Info)
```

### Buttons

```csharp
// Standard buttons
.WithButtons(MessageBoxButtons.YesNoCancel)

// Custom buttons
.WithCustomButtons(
    new MessageBoxButtonDefinition 
    { 
        Text = "Save", 
        Result = MessageBoxResult.Yes,
        Style = ButtonStyle.Primary,
        IsDefault = true 
    },
    new MessageBoxButtonDefinition 
    { 
        Text = "Discard", 
        Result = MessageBoxResult.No,
        Style = ButtonStyle.Danger 
    },
    new MessageBoxButtonDefinition 
    { 
        Text = "Cancel", 
        Result = MessageBoxResult.Cancel,
        IsCancel = true 
    }
)
```

### Input

```csharp
.WithInput(
    placeholder: "Enter value",
    defaultValue: "Default",
    isPassword: false
)
```

### Checkbox

```csharp
.WithCheckbox(
    text: "Don't show this again",
    isChecked: false
)
```

### Dropdown

```csharp
var items = new[] { "Option A", "Option B", "Option C" };
.WithDropdown(items, displayMemberPath: null, defaultIndex: 0)
```

### Resizable

```csharp
// Make the dialog resizable (user can drag edges)
.WithResizable()
```

### Custom Content

```csharp
.WithContent(new CheckBox { Content = "Enable feature" })
```

### Appearance

```csharp
.WithTitleBarColor(Colors.DarkBlue)
.WithSize(width: 500, height: 300)
.WithOwner(Application.Current.MainWindow)
```

---

## Callbacks

Register callbacks to execute based on the user's response.

### Button Callbacks

```csharp
await MessageBoxBuilder.Create(_messageBox)
    .WithTitle("Save Changes")
    .WithMessage("Do you want to save before closing?")
    .WithButtons(MessageBoxButtons.YesNoCancel)
    .OnYes(() => {
        SaveDocument();
        CloseWindow();
    })
    .OnNo(() => {
        CloseWindow();
    })
    .OnCancel(() => {
        // Stay open
    })
    .ShowAsync();
```

### OK Callback

```csharp
await _messageBox.Info("Settings applied.")
    .OnOk(() => RefreshUI())
    .ShowAsync();
```

### Result Callback

Get the full `DialogResult` for extended data:

```csharp
await MessageBoxBuilder.Create(_messageBox)
    .WithTitle("Confirm")
    .WithMessage("Delete item?")
    .WithButtons(MessageBoxButtons.YesNo)
    .WithCheckbox("Don't ask again")
    .OnResult(result => {
        if (result.IsChecked)
        {
            Settings.DontAskOnDelete = true;
        }
    })
    .OnYes(() => DeleteItem())
    .ShowAsync();
```

---

## Common Patterns

### Confirmation with Action

```csharp
public async Task DeleteSelectedAsync()
{
    await _messageBox.Confirm($"Delete {SelectedItem.Name}?", "Confirm Delete")
        .OnYes(async () => 
        {
            await _repository.DeleteAsync(SelectedItem);
            Items.Remove(SelectedItem);
            _toast.ShowSuccess("Item deleted.");
        })
        .ShowAsync();
}
```

### Input Dialog

```csharp
public async Task RenameAsync()
{
    var result = await MessageBoxBuilder.Create(_messageBox)
        .WithTitle("Rename")
        .WithMessage("Enter new name:")
        .WithInput("Name", SelectedItem.Name)
        .WithButtons(MessageBoxButtons.OKCancel)
        .ShowAsync();

    if (result.Result == MessageBoxResult.OK && !string.IsNullOrEmpty(result.InputText))
    {
        SelectedItem.Name = result.InputText;
        await _repository.UpdateAsync(SelectedItem);
    }
}
```

### Save Confirmation

```csharp
public async Task<bool> ConfirmCloseAsync()
{
    if (!HasUnsavedChanges) return true;

    var result = await MessageBoxBuilder.Create(_messageBox)
        .WithTitle("Unsaved Changes")
        .WithMessage("You have unsaved changes. What would you like to do?")
        .WithIcon(MessageBoxIcon.Warning)
        .WithCustomButtons(
            new() { Text = "Save", Result = MessageBoxResult.Yes, Style = ButtonStyle.Primary, IsDefault = true },
            new() { Text = "Don't Save", Result = MessageBoxResult.No },
            new() { Text = "Cancel", Result = MessageBoxResult.Cancel, IsCancel = true }
        )
        .ShowAsync();

    switch (result.Result)
    {
        case MessageBoxResult.Yes:
            await SaveAsync();
            return true;
        case MessageBoxResult.No:
            return true;
        default:
            return false;
    }
}
```

### Don't Show Again

```csharp
public async Task ShowTipAsync()
{
    if (Settings.HideTip) return;

    await MessageBoxBuilder.Create(_messageBox)
        .WithTitle("Tip")
        .WithMessage("You can press Ctrl+S to save quickly.")
        .WithIcon(MessageBoxIcon.Info)
        .WithButtons(MessageBoxButtons.OK)
        .WithCheckbox("Don't show this again")
        .OnResult(result => {
            if (result.IsChecked)
            {
                Settings.HideTip = true;
            }
        })
        .ShowAsync();
}
```

---

## Building Without Showing

Use `BuildOptions()` to get configured options without displaying:

```csharp
var options = MessageBoxBuilder.Create(_messageBox)
    .WithTitle("Confirm")
    .WithMessage("Proceed?")
    .WithButtons(MessageBoxButtons.YesNo)
    .BuildOptions();

// Use options elsewhere
await _messageBox.ShowAsync(options);
```

---

## Async Callbacks

For async operations in callbacks, use async lambdas:

```csharp
await _messageBox.Confirm("Delete?")
    .OnYes(async () => 
    {
        await _repository.DeleteAsync(item);
        await RefreshListAsync();
    })
    .ShowAsync();
```

Note: The dialog closes immediately after the button is clicked; the callback runs after.

---

## Best Practices

1. **Use extension methods** - `Confirm()`, `Info()`, etc. are shorter than `Create()`
2. **Chain fluently** - Keep the configuration readable
3. **Handle all cases** - Consider what happens on Cancel/close
4. **Keep callbacks simple** - Complex logic belongs in separate methods
5. **Use OnResult for extended data** - When you need checkbox state or input text
