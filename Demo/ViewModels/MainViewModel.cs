using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
    /// Applies the current theme based on IsDarkMode value.
    /// </summary>
    /// <remarks>
    /// Theme changes take effect immediately for all subsequent dialogs.
    /// Note: IsDarkMode is already toggled by the ToggleButton's IsChecked binding.
    /// </remarks>
    [RelayCommand]
    private void ToggleTheme()
    {
        // IsDarkMode is already set by the ToggleButton's IsChecked two-way binding
        // We just need to apply the theme based on the current value
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

    #region Native Dialog Commands

    /// <summary>
    /// Shows the native checkbox dialog.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativeCheckboxAsync()
    {
        var dialogResult = await _messageBoxService.ConfirmWithCheckboxAsync(
            "Do you want to save changes before closing?",
            "Don't ask me again",
            "Save Changes"
        );

        LastResult = $"Checkbox dialog: Result={dialogResult.Result}, Checked={dialogResult.IsChecked}";
    }

    /// <summary>
    /// Shows the native input dialog.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativeInputAsync()
    {
        var dialogResult = await _messageBoxService.InputAsync(
            "Please enter your name:",
            "Enter name here...",
            string.Empty,
            "User Input"
        );

        LastResult = $"Input dialog: Result={dialogResult.Result}, Text=\"{dialogResult.InputText}\"";
    }

    /// <summary>
    /// Shows the native password input dialog.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativePasswordAsync()
    {
        var dialogResult = await _messageBoxService.InputAsync(
            "Please enter your password:",
            "Password",
            string.Empty,
            "Authentication",
            isPassword: true
        );

        LastResult = $"Password dialog: Result={dialogResult.Result}, Length={dialogResult.InputText?.Length ?? 0}";
    }

    /// <summary>
    /// Shows the native selection dialog.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativeSelectionAsync()
    {
        var items = new[] { "Option A", "Option B", "Option C", "Option D" };
        var dialogResult = await _messageBoxService.SelectAsync(
            "Please select an option:",
            items,
            defaultIndex: 1,
            title: "Selection"
        );

        LastResult = $"Selection dialog: Result={dialogResult.Result}, Selected=\"{dialogResult.SelectedItem}\", Index={dialogResult.SelectedIndex}";
    }

    /// <summary>
    /// Shows the native license dialog.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativeLicenseAsync()
    {
        const string licenseText = """
            SOFTWARE LICENSE AGREEMENT
            
            This Software License Agreement ("Agreement") is entered into as of the date of acceptance by the user ("Licensee") and the software provider ("Licensor").
            
            1. GRANT OF LICENSE
            Subject to the terms of this Agreement, Licensor grants to Licensee a non-exclusive, non-transferable, limited license to use the software.
            
            2. RESTRICTIONS
            Licensee shall not: (a) modify, translate, adapt, or create derivative works based upon the software; (b) reverse engineer, disassemble, decompile, or otherwise attempt to derive the source code of the software.
            
            3. INTELLECTUAL PROPERTY
            The software is protected by copyright and other intellectual property laws. Licensor retains all rights not expressly granted.
            
            4. DISCLAIMER OF WARRANTIES
            THE SOFTWARE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED.
            
            5. LIMITATION OF LIABILITY
            IN NO EVENT SHALL LICENSOR BE LIABLE FOR ANY INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES.
            
            6. TERMINATION
            This Agreement is effective until terminated. Licensor may terminate this Agreement immediately if Licensee breaches any provision.
            
            7. GENERAL PROVISIONS
            This Agreement constitutes the entire agreement between the parties concerning the subject matter hereof.
            
            By clicking "OK", you acknowledge that you have read and agree to the terms of this license agreement.
            
            --- END OF LICENSE ---
            """;

        var dialogResult = await _messageBoxService.LicenseAsync(
            "License Agreement",
            "Please read and scroll to the bottom to accept the license agreement:",
            licenseText,
            requireScrollToBottom: true
        );

        LastResult = $"License dialog: Result={dialogResult.Result} (accepted={dialogResult.Result == MessageBoxResult.OK})";
    }

    /// <summary>
    /// Shows the native timeout dialog.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativeTimeoutAsync()
    {
        var dialogResult = await _messageBoxService.TimeoutAsync(
            "This dialog will automatically close in 5 seconds unless you click OK.",
            timeoutSeconds: 5,
            timeoutResult: MessageBoxResult.Cancel,
            title: "Auto-Close Demo"
        );

        LastResult = $"Timeout dialog: Result={dialogResult.Result}, TimedOut={dialogResult.TimedOut}";
    }

    /// <summary>
    /// Shows a warning dialog using the convenience method.
    /// </summary>
    [RelayCommand]
    private async Task ShowWarningAsync()
    {
        var result = await _messageBoxService.WarningAsync(
            "This action may have unintended consequences. Please proceed with caution.",
            "Warning"
        );

        LastResult = $"WarningAsync returned: {result}";
    }

    /// <summary>
    /// Shows a dialog with custom sizing.
    /// </summary>
    [RelayCommand]
    private async Task ShowCustomSizedDialogAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "Custom Sized Dialog",
            Message = "This dialog demonstrates custom width, height, and title bar color. The dialog is wider and taller than the default size.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK,
            Width = 600,
            Height = 400,
            TitleBarColor = Colors.DarkBlue
        };

        var result = await _messageBoxService.ShowAsync(options);
        LastResult = $"Custom sized dialog returned: {result}";
    }

    /// <summary>
    /// Shows a license dialog with improved layout.
    /// </summary>
    [RelayCommand]
    private async Task ShowImprovedLicenseAsync()
    {
        const string improvedLicense = """
            SOFTWARE LICENSE AGREEMENT
            ========================
            
            This Software License Agreement ("Agreement") is entered into as of the date of acceptance by the user ("Licensee") and the software provider ("Licensor").
            
            1. GRANT OF LICENSE
            -------------------
            Subject to the terms and conditions of this Agreement, Licensor hereby grants to Licensee a non-exclusive, non-transferable, limited license to use, install, and operate the software solely for Licensee's internal business purposes.
            
            2. RESTRICTIONS
            ---------------
            Licensee shall not, and shall not permit any third party to: 
            (a) modify, translate, adapt, or create derivative works based upon the software; 
            (b) reverse engineer, disassemble, decompile, or otherwise attempt to derive the source code of the software; 
            (c) distribute, sublicense, rent, lease, or lend the software to any third party.
            
            3. INTELLECTUAL PROPERTY RIGHTS
            --------------------------------
            The software and all intellectual property rights therein are and shall remain the exclusive property of Licensor. Licensor retains all rights not expressly granted to Licensee under this Agreement.
            
            4. DISCLAIMER OF WARRANTIES
            ---------------------------
            THE SOFTWARE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT.
            
            5. LIMITATION OF LIABILITY
            --------------------------
            IN NO EVENT SHALL LICENSOR BE LIABLE FOR ANY INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY.
            
            6. TERMINATION
            --------------
            This Agreement is effective until terminated. Licensor may terminate this Agreement immediately upon notice if Licensee breaches any provision of this Agreement.
            
            7. GENERAL PROVISIONS
            ---------------------
            This Agreement constitutes the entire agreement between the parties concerning the subject matter hereof and supersedes all prior or contemporaneous agreements.
            
            By clicking "Accept", you acknowledge that you have read and understand the terms of this license agreement and agree to be bound by its terms and conditions.
            
            --- END OF LICENSE AGREEMENT ---
            """;

        var options = new MessageBoxOptions
        {
            Title = "Software License Agreement",
            Message = "Please read the license agreement carefully. You must scroll to the bottom before accepting.",
            DetailedText = improvedLicense,
            RequireScrollToBottom = true,
            Buttons = MessageBoxButtons.OKCancel,
            Icon = MessageBoxIcon.Info,
            Width = 700,
            Height = 500,
            TitleBarColor = Colors.DarkGreen
        };

        var result = await _messageBoxService.ShowExtendedAsync(options);
        LastResult = $"Improved license dialog: Result={result.Result}, Accepted={result.Result == MessageBoxResult.OK}";
    }

    #endregion
}