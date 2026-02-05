# Progress Dialogs

Guide to displaying progress dialogs for long-running operations.

## Basic Usage

### Manual Control

Get a controller to manually update progress:

```csharp
var controller = await _messageBox.ShowProgressAsync(new ProgressOptions
{
    Title = "Processing",
    Message = "Please wait...",
    IsCancellable = true
});

try
{
    for (int i = 0; i <= 100; i++)
    {
        // Check for cancellation
        if (controller.IsCancellationRequested)
        {
            break;
        }

        // Update progress
        controller.SetProgress(i);
        controller.SetMessage($"Processing item {i} of 100...");

        await Task.Delay(50); // Simulate work
    }
}
finally
{
    // Always close the dialog
    await controller.CloseAsync();
}
```

### Automatic Progress

Use `RunWithProgressAsync` for simpler scenarios:

```csharp
var result = await _messageBox.RunWithProgressAsync(
    async (progress, cancellationToken) =>
    {
        for (int i = 0; i <= 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress.Report(i);
            await Task.Delay(50);
        }
        return "Operation completed!";
    },
    new ProgressOptions
    {
        Title = "Processing",
        Message = "Working...",
        IsCancellable = true
    }
);

// result is "Operation completed!" or null if cancelled
```

---

## Progress Options

### ProgressOptions Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | `string` | `"Progress"` | Dialog title |
| `Message` | `string` | `""` | Initial status message |
| `IsIndeterminate` | `bool` | `true` | Show spinning animation vs percentage |
| `IsCancellable` | `bool` | `true` | Show cancel button |
| `ShowPercentage` | `bool` | `true` | Display percentage text |
| `InitialProgress` | `double` | `0` | Starting progress value |
| `Owner` | `Window?` | `null` | Owner window |
| `Width` | `double?` | `null` | Fixed dialog width |

### Indeterminate Progress

For operations with unknown duration:

```csharp
var controller = await _messageBox.ShowProgressAsync(new ProgressOptions
{
    Title = "Loading",
    Message = "Fetching data from server...",
    IsIndeterminate = true,  // Shows spinning animation
    IsCancellable = false    // No cancel button
});

try
{
    await FetchDataAsync();
}
finally
{
    await controller.CloseAsync();
}
```

### Determinate Progress

For operations with known progress:

```csharp
var controller = await _messageBox.ShowProgressAsync(new ProgressOptions
{
    Title = "Downloading",
    Message = "Starting download...",
    IsIndeterminate = false,  // Shows percentage bar
    ShowPercentage = true,    // Display "50%" text
    IsCancellable = true
});
```

---

## IProgressController

The controller interface for managing progress dialogs.

### Properties

```csharp
// Check if user clicked Cancel
if (controller.IsCancellationRequested)
{
    // Clean up and exit
}

// Use CancellationToken with async operations
await httpClient.GetAsync(url, controller.CancellationToken);
```

### Methods

```csharp
// Update progress (0-100)
controller.SetProgress(50);

// Update message
controller.SetMessage("Processing file 5 of 10...");

// Switch between indeterminate and determinate
controller.SetIndeterminate(false);

// Close dialog when done
await controller.CloseAsync();
```

---

## Common Patterns

### File Processing

```csharp
public async Task ProcessFilesAsync(IEnumerable<string> files)
{
    var fileList = files.ToList();
    var controller = await _messageBox.ShowProgressAsync(new ProgressOptions
    {
        Title = "Processing Files",
        Message = "Starting...",
        IsIndeterminate = false,
        IsCancellable = true
    });

    try
    {
        for (int i = 0; i < fileList.Count; i++)
        {
            if (controller.IsCancellationRequested)
            {
                _toast.ShowWarning("Operation cancelled.");
                break;
            }

            var file = fileList[i];
            controller.SetMessage($"Processing {Path.GetFileName(file)}...");
            controller.SetProgress((double)(i + 1) / fileList.Count * 100);

            await ProcessFileAsync(file, controller.CancellationToken);
        }

        if (!controller.IsCancellationRequested)
        {
            _toast.ShowSuccess($"Processed {fileList.Count} files.");
        }
    }
    finally
    {
        await controller.CloseAsync();
    }
}
```

### Download with Progress

```csharp
public async Task<byte[]?> DownloadWithProgressAsync(string url)
{
    return await _messageBox.RunWithProgressAsync(
        async (progress, cancellationToken) =>
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, 
                HttpCompletionOption.ResponseHeadersRead, 
                cancellationToken);
            
            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var buffer = new byte[8192];
            var totalRead = 0L;

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var memoryStream = new MemoryStream();

            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                totalRead += bytesRead;

                if (totalBytes > 0)
                {
                    progress.Report((double)totalRead / totalBytes * 100);
                }
            }

            return memoryStream.ToArray();
        },
        new ProgressOptions
        {
            Title = "Downloading",
            Message = "Downloading file...",
            IsIndeterminate = false,
            IsCancellable = true
        }
    );
}
```

### Multi-Step Operation

```csharp
public async Task PerformSetupAsync()
{
    var controller = await _messageBox.ShowProgressAsync(new ProgressOptions
    {
        Title = "Setup",
        Message = "Initializing...",
        IsIndeterminate = false
    });

    try
    {
        // Step 1
        controller.SetMessage("Checking prerequisites...");
        controller.SetProgress(0);
        await CheckPrerequisitesAsync();

        // Step 2
        controller.SetMessage("Downloading components...");
        controller.SetProgress(25);
        await DownloadComponentsAsync();

        // Step 3
        controller.SetMessage("Installing...");
        controller.SetProgress(50);
        await InstallAsync();

        // Step 4
        controller.SetMessage("Configuring...");
        controller.SetProgress(75);
        await ConfigureAsync();

        // Complete
        controller.SetMessage("Finalizing...");
        controller.SetProgress(100);
        await Task.Delay(500);
    }
    finally
    {
        await controller.CloseAsync();
    }
}
```

---

## Error Handling

### Handle Cancellation

```csharp
var controller = await _messageBox.ShowProgressAsync(options);

try
{
    await DoWorkAsync(controller.CancellationToken);
}
catch (OperationCanceledException)
{
    // User cancelled - this is expected
    _toast.ShowInfo("Operation cancelled.");
}
catch (Exception ex)
{
    await _messageBox.ErrorAsync("Operation failed.", ex);
}
finally
{
    await controller.CloseAsync();
}
```

### Using RunWithProgressAsync

Cancellation is handled automatically:

```csharp
var result = await _messageBox.RunWithProgressAsync(
    async (progress, ct) =>
    {
        // OperationCanceledException is caught and returns null/default
        await SomeOperationAsync(ct);
        return "Done";
    },
    options
);

if (result == null)
{
    // Was cancelled
}
```

---

## Best Practices

1. **Always close the dialog** - Use try/finally to ensure `CloseAsync()` is called
2. **Check cancellation frequently** - Especially in loops
3. **Update messages** - Give users feedback about what's happening
4. **Use appropriate mode** - Indeterminate for unknown duration, determinate when you can calculate progress
5. **Handle errors gracefully** - Close progress dialog before showing error dialog
