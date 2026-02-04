using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;

namespace FluentDialogs.Demo.ViewModels;

/// <summary>
/// Main ViewModel demonstrating all FluentDialogs.Wpf features.
/// </summary>
/// <remarks>
/// This ViewModel showcases the recommended patterns for using FluentDialogs
/// in an MVVM application with dependency injection.
/// </remarks>
public partial class MainViewModel : ObservableObject
{
    private readonly IMessageBoxService _messageBoxService;
    private readonly IMessageBoxThemeService _themeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="messageBoxService">The message box service for displaying dialogs.</param>
    /// <param name="themeService">The theme service for managing dialog themes.</param>
    /// <remarks>
    /// Both services are injected via constructor injection.
    /// This is the recommended pattern for FluentDialogs integration.
    /// </remarks>
    public MainViewModel(
        IMessageBoxService messageBoxService,
        IMessageBoxThemeService themeService)
    {
        _messageBoxService = messageBoxService;
        _themeService = themeService;
    }

    /// <summary>
    /// Gets the current theme name for display.
    /// </summary>
    [ObservableProperty]
    private string _currentTheme = "Light";

    /// <summary>
    /// Gets or sets whether dark mode is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _isDarkMode;

    /// <summary>
    /// Gets the last dialog result for display.
    /// </summary>
    [ObservableProperty]
    private string _lastResult = "No dialog shown yet";

    #region Theme Commands

    /// <summary>
    /// Toggles between Light and Dark themes.
    /// </summary>
    /// <remarks>
    /// Theme changes take effect immediately for all subsequent dialogs.
    /// </remarks>
    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        var newTheme = IsDarkMode ? MessageBoxTheme.Dark : MessageBoxTheme.Light;

        _themeService.SetTheme(newTheme);
        CurrentTheme = newTheme.ToString();
    }

    #endregion

    #region Standard Dialog Commands

    /// <summary>
    /// Shows a simple info dialog using the convenience method.
    /// </summary>
    [RelayCommand]
    private async Task ShowInfoAsync()
    {
        // InfoAsync is a convenience method for simple informational messages
        var result = await _messageBoxService.InfoAsync(
            "This is an informational message displayed using the InfoAsync convenience method.",
            "Information"
        );

        LastResult = $"InfoAsync returned: {result}";
    }

    /// <summary>
    /// Shows a confirmation dialog using the convenience method.
    /// </summary>
    [RelayCommand]
    private async Task ShowConfirmAsync()
    {
        // ConfirmAsync provides Yes/No buttons by default
        var result = await _messageBoxService.ConfirmAsync(
            "Do you want to proceed with this action?",
            "Confirm Action"
        );

        LastResult = $"ConfirmAsync returned: {result}";
    }

    /// <summary>
    /// Shows an error dialog using the convenience method.
    /// </summary>
    [RelayCommand]
    private async Task ShowErrorAsync()
    {
        // ErrorAsync displays an error message with OK button
        var result = await _messageBoxService.ErrorAsync(
            "An error occurred while processing your request. Please try again."
        );

        LastResult = $"ErrorAsync returned: {result}";
    }

    /// <summary>
    /// Shows an error dialog with exception details.
    /// </summary>
    [RelayCommand]
    private async Task ShowErrorWithExceptionAsync()
    {
        // Create a sample exception to demonstrate exception display
        Exception sampleException;
        try
        {
            throw new InvalidOperationException(
                "This is a sample exception message.",
                new ArgumentNullException("parameter", "Inner exception details")
            );
        }
        catch (Exception ex)
        {
            sampleException = ex;
        }

        // ErrorAsync with exception parameter shows expandable stack trace
        var result = await _messageBoxService.ErrorAsync(
            "An unexpected error occurred. Click 'Show Details' to see the stack trace.",
            sampleException
        );

        LastResult = $"ErrorAsync (with exception) returned: {result}";
    }

    #endregion

    #region Button Configuration Commands

    /// <summary>
    /// Shows a dialog with OK button only.
    /// </summary>
    [RelayCommand]
    private async Task ShowOkAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "OK Button",
            Message = "This dialog has only an OK button.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"OK dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with OK and Cancel buttons.
    /// </summary>
    [RelayCommand]
    private async Task ShowOkCancelAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "OK / Cancel",
            Message = "This dialog has OK and Cancel buttons.",
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.OKCancel
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"OKCancel dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with Yes and No buttons.
    /// </summary>
    [RelayCommand]
    private async Task ShowYesNoAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Yes / No",
            Message = "This dialog has Yes and No buttons.",
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.YesNo
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"YesNo dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with Yes, No, and Cancel buttons.
    /// </summary>
    [RelayCommand]
    private async Task ShowYesNoCancelAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Yes / No / Cancel",
            Message = "This dialog has Yes, No, and Cancel buttons.",
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.YesNoCancel
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"YesNoCancel dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with Retry and Cancel buttons.
    /// </summary>
    [RelayCommand]
    private async Task ShowRetryCancelAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Retry / Cancel",
            Message = "This dialog has Retry and Cancel buttons. Useful for error recovery scenarios.",
            Icon = MessageBoxIcon.Warning,
            Buttons = MessageBoxButtons.RetryCancel
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"RetryCancel dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with Abort, Retry, and Ignore buttons.
    /// </summary>
    [RelayCommand]
    private async Task ShowAbortRetryIgnoreAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Abort / Retry / Ignore",
            Message = "This dialog has Abort, Retry, and Ignore buttons. Useful for batch operation error handling.",
            Icon = MessageBoxIcon.Error,
            Buttons = MessageBoxButtons.AbortRetryIgnore
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"AbortRetryIgnore dialog returned: {result}";
    }

    #endregion

    #region Icon Type Commands

    /// <summary>
    /// Shows a dialog with no icon.
    /// </summary>
    [RelayCommand]
    private async Task ShowNoIconAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "No Icon",
            Message = "This dialog has no icon displayed.",
            Icon = MessageBoxIcon.None,
            Buttons = MessageBoxButtons.OK
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"No icon dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with info icon.
    /// </summary>
    [RelayCommand]
    private async Task ShowInfoIconAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Info Icon",
            Message = "This dialog displays the Info icon, typically used for informational messages.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Info icon dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with warning icon.
    /// </summary>
    [RelayCommand]
    private async Task ShowWarningIconAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Warning Icon",
            Message = "This dialog displays the Warning icon, typically used for cautionary messages.",
            Icon = MessageBoxIcon.Warning,
            Buttons = MessageBoxButtons.OK
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Warning icon dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with error icon.
    /// </summary>
    [RelayCommand]
    private async Task ShowErrorIconAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Error Icon",
            Message = "This dialog displays the Error icon, typically used for error messages.",
            Icon = MessageBoxIcon.Error,
            Buttons = MessageBoxButtons.OK
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Error icon dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with success icon.
    /// </summary>
    [RelayCommand]
    private async Task ShowSuccessIconAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Success Icon",
            Message = "This dialog displays the Success icon, typically used for completion messages.",
            Icon = MessageBoxIcon.Success,
            Buttons = MessageBoxButtons.OK
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Success icon dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with question icon.
    /// </summary>
    [RelayCommand]
    private async Task ShowQuestionIconAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Question Icon",
            Message = "This dialog displays the Question icon, typically used for confirmation prompts.",
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.YesNo
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Question icon dialog returned: {result}";
    }

    #endregion

    #region Custom Button Commands

    /// <summary>
    /// Shows a dialog with custom Save/Don't Save/Cancel buttons.
    /// </summary>
    /// <remarks>
    /// This demonstrates how to create custom buttons with different styles.
    /// </remarks>
    [RelayCommand]
    private async Task ShowCustomButtonsAsync()
    {
        // Define custom buttons with specific styles
        var customButtons = new List<MessageBoxButtonDefinition>
        {
            new()
            {
                Text = "Save",
                Result = MessageBoxResult.Yes,
                Style = ButtonStyle.Primary,  // Primary style for main action
                IsDefault = true              // Enter key triggers this button
            },
            new()
            {
                Text = "Don't Save",
                Result = MessageBoxResult.No,
                Style = ButtonStyle.Danger    // Danger style for destructive action
            },
            new()
            {
                Text = "Cancel",
                Result = MessageBoxResult.Cancel,
                Style = ButtonStyle.Default,  // Default style for cancel
                IsCancel = true               // Escape key triggers this button
            }
        };

        var options = new MessageBoxOptions
        {
            Title = "Unsaved Changes",
            Message = "You have unsaved changes. What would you like to do?",
            Icon = MessageBoxIcon.Warning,
            CustomButtons = customButtons
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Custom buttons dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with custom buttons that execute commands before closing.
    /// </summary>
    /// <remarks>
    /// This demonstrates how to attach custom commands to buttons.
    /// The command executes before the dialog closes.
    /// </remarks>
    [RelayCommand]
    private async Task ShowCustomButtonsWithCommandsAsync()
    {
        // Create commands that will execute when buttons are clicked
        var openSettingsCommand = new RelayCommand(() =>
        {
            Debug.WriteLine("Open Settings command executed!");
        });

        var learnMoreCommand = new RelayCommand(() =>
        {
            Debug.WriteLine("Learn More command executed!");
        });

        var customButtons = new List<MessageBoxButtonDefinition>
        {
            new()
            {
                Text = "Open Settings",
                Result = MessageBoxResult.Yes,
                Style = ButtonStyle.Primary,
                IsDefault = true,
                Command = openSettingsCommand  // Command executes before dialog closes
            },
            new()
            {
                Text = "Learn More",
                Result = MessageBoxResult.No,
                Style = ButtonStyle.Secondary,
                Command = learnMoreCommand
            },
            new()
            {
                Text = "Not Now",
                Result = MessageBoxResult.Cancel,
                Style = ButtonStyle.Default,
                IsCancel = true
            }
        };

        var options = new MessageBoxOptions
        {
            Title = "Feature Available",
            Message = "A new feature is available. Would you like to configure it now?",
            Icon = MessageBoxIcon.Info,
            CustomButtons = customButtons
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Custom buttons with commands returned: {result} (check Debug output)";
    }

    #endregion

    #region Custom Content Commands

    /// <summary>
    /// Shows a dialog with a checkbox as custom content.
    /// </summary>
    [RelayCommand]
    private async Task ShowWithCheckboxAsync()
    {
        // Create a checkbox to embed in the dialog
        var checkBox = new CheckBox
        {
            Content = "Don't show this message again",
            Margin = new Thickness(0, 8, 0, 0),
            FontSize = 13
        };

        var options = new MessageBoxOptions
        {
            Title = "Important Notice",
            Message = "This is an important notice that you should read carefully before proceeding.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OKCancel,
            Content = checkBox  // Custom content appears below the message
        };

        var result = await _messageBoxService.ShowAsync(options);

        // Access the checkbox state after dialog closes
        var wasChecked = checkBox.IsChecked == true;
        LastResult = $"Checkbox dialog returned: {result}, Checkbox checked: {wasChecked}";
    }

    /// <summary>
    /// Shows a dialog with a hyperlink as custom content.
    /// </summary>
    [RelayCommand]
    private async Task ShowWithHyperlinkAsync()
    {
        // Create a hyperlink that opens a URL
        var hyperlink = new Hyperlink(new Run("Learn more about FluentDialogs"))
        {
            NavigateUri = new Uri("https://github.com/LongKelvin/FluentDialogs.Wpf")
        };
        hyperlink.RequestNavigate += (s, e) =>
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        };

        var textBlock = new TextBlock
        {
            Margin = new Thickness(0, 8, 0, 0),
            FontSize = 13
        };
        textBlock.Inlines.Add(hyperlink);

        var options = new MessageBoxOptions
        {
            Title = "Welcome",
            Message = "Thank you for using FluentDialogs.Wpf! Click the link below for documentation.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK,
            Content = textBlock  // TextBlock with hyperlink as custom content
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Hyperlink dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with a progress bar as custom content.
    /// </summary>
    [RelayCommand]
    private async Task ShowWithProgressAsync()
    {
        var progressBar = new ProgressBar
        {
            IsIndeterminate = true,
            Height = 4,
            Margin = new Thickness(0, 16, 0, 0)
        };

        var options = new MessageBoxOptions
        {
            Title = "Processing",
            Message = "Please wait while the operation completes...",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK,
            Content = progressBar
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Progress dialog returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with a text input as custom content.
    /// </summary>
    [RelayCommand]
    private async Task ShowWithInputAsync()
    {
        var stackPanel = new StackPanel { Margin = new Thickness(0, 12, 0, 0) };
        
        var label = new TextBlock
        {
            Text = "Enter your name:",
            FontSize = 13,
            Margin = new Thickness(0, 0, 0, 8)
        };
        
        var textBox = new TextBox
        {
            Height = 32,
            FontSize = 14,
            Padding = new Thickness(8, 4, 8, 4)
        };
        
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(textBox);

        var options = new MessageBoxOptions
        {
            Title = "Input Required",
            Message = "We need some information from you.",
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.OKCancel,
            Content = stackPanel
        };

        var result = await _messageBoxService.ShowAsync(options);
        var inputValue = textBox.Text;
        LastResult = $"Input dialog returned: {result}, Input: \"{inputValue}\"";
    }

    /// <summary>
    /// Shows a delete confirmation dialog with danger button.
    /// </summary>
    [RelayCommand]
    private async Task ShowDeleteConfirmationAsync()
    {
        var customButtons = new List<MessageBoxButtonDefinition>
        {
            new()
            {
                Text = "Delete",
                Result = MessageBoxResult.Yes,
                Style = ButtonStyle.Danger,
                IsDefault = false
            },
            new()
            {
                Text = "Cancel",
                Result = MessageBoxResult.Cancel,
                Style = ButtonStyle.Default,
                IsCancel = true,
                IsDefault = true
            }
        };

        var options = new MessageBoxOptions
        {
            Title = "Delete Item",
            Message = "Are you sure you want to delete this item? This action cannot be undone.",
            Icon = MessageBoxIcon.Warning,
            CustomButtons = customButtons
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Delete confirmation returned: {result}";
    }

    #endregion
}

