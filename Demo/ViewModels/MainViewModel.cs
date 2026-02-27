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
/// <para>
/// This ViewModel showcases the recommended patterns for using FluentDialogs
/// in an MVVM application with dependency injection, including:
/// </para>
/// <list type="bullet">
/// <item><description>Message box dialogs with various button and icon configurations</description></item>
/// <item><description>Progress dialogs with cancellation support</description></item>
/// <item><description>Toast notifications for non-blocking feedback</description></item>
/// <item><description>Fluent builder API for readable dialog construction</description></item>
/// <item><description>Theme switching with ThemeChanged event subscription</description></item>
/// </list>
/// </remarks>
public partial class MainViewModel : ObservableObject
{
    private readonly IMessageBoxService _messageBoxService;
    private readonly IMessageBoxThemeService _themeService;
    private readonly IToastService _toastService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="messageBoxService">The message box service for displaying dialogs.</param>
    /// <param name="themeService">The theme service for managing dialog themes.</param>
    /// <param name="toastService">The toast service for displaying non-blocking notifications.</param>
    /// <remarks>
    /// All services are injected via constructor injection.
    /// This is the recommended pattern for FluentDialogs integration.
    /// </remarks>
    public MainViewModel(
        IMessageBoxService messageBoxService,
        IMessageBoxThemeService themeService,
        IToastService toastService)
    {
        _messageBoxService = messageBoxService;
        _themeService = themeService;
        _toastService = toastService;

        // Subscribe to theme changes to react when theme is updated
        _themeService.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// Handles the ThemeChanged event from the theme service.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments containing old and new theme.</param>
    /// <remarks>
    /// This demonstrates how to subscribe to theme changes and react accordingly.
    /// Useful for updating UI elements or showing notifications when theme changes.
    /// </remarks>
    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        // Update the current theme display
        CurrentTheme = e.NewTheme.ToString();

        // Show a toast notification about the theme change
        _toastService.ShowInfo($"Theme changed to {e.NewTheme}");
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
    /// Shows the native dropdown dialog with a ComboBox.
    /// </summary>
    [RelayCommand]
    private async Task ShowNativeDropdownAsync()
    {
        var items = new[] { "English", "French", "German", "Spanish", "Japanese", "Chinese" };
        var dialogResult = await _messageBoxService.DropdownAsync(
            "Select your preferred language:",
            items,
            defaultIndex: 0,
            title: "Language Selection"
        );

        LastResult = $"Dropdown dialog: Result={dialogResult.Result}, Selected=\"{dialogResult.DropdownSelectedItem}\", Index={dialogResult.DropdownSelectedIndex}";
    }

    /// <summary>
    /// Shows a dropdown dialog using the builder API.
    /// </summary>
    [RelayCommand]
    private async Task ShowDropdownBuilderAsync()
    {
        var colors = new[] { "Red", "Green", "Blue", "Purple", "Orange" };
        var result = await _messageBoxService.Dropdown("Choose a color:", colors, defaultIndex: 2, title: "Color Picker")
            .OnOk(() => _toastService.ShowSuccess("Color selected!"))
            .OnCancel(() => _toastService.ShowInfo("Selection cancelled"))
            .ShowAsync();

        LastResult = $"Builder dropdown: Result={result.Result}, Selected=\"{result.DropdownSelectedItem}\"";
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
            TitleBarColor = Colors.DarkGreen,
            IsResizable = true
        };

        var result = await _messageBoxService.ShowExtendedAsync(options);
        LastResult = $"Improved license dialog: Result={result.Result}, Accepted={result.Result == MessageBoxResult.OK}";
    }

    #endregion

    #region Progress Dialog Commands

    /// <summary>
    /// Shows an indeterminate progress dialog.
    /// </summary>
    /// <remarks>
    /// Demonstrates the ShowProgressAsync method for displaying
    /// a progress dialog without knowing the total work amount.
    /// </remarks>
    [RelayCommand]
    private async Task ShowProgressIndeterminateAsync()
    {
        var options = new ProgressOptions
        {
            Title = "Processing",
            Message = "Please wait while we process your request...",
            IsIndeterminate = true,
            IsCancellable = true
        };

        var progress = await _messageBoxService.ShowProgressAsync(options);

        try
        {
            // Simulate work with cancellation support
            for (int i = 0; i < 50; i++)
            {
                if (progress.IsCancellationRequested)
                {
                    LastResult = "Progress dialog: Cancelled by user";
                    return;
                }

                await Task.Delay(100, progress.CancellationToken);
            }

            LastResult = "Progress dialog: Completed successfully";
        }
        catch (OperationCanceledException)
        {
            LastResult = "Progress dialog: Cancelled by user";
        }
        finally
        {
            await progress.CloseAsync();
        }
    }

    /// <summary>
    /// Shows a determinate progress dialog with percentage display.
    /// </summary>
    /// <remarks>
    /// Demonstrates progress reporting with percentage updates
    /// and dynamic message changes.
    /// </remarks>
    [RelayCommand]
    private async Task ShowProgressDeterminateAsync()
    {
        var options = new ProgressOptions
        {
            Title = "Downloading Files",
            Message = "Preparing download...",
            IsIndeterminate = false,
            IsCancellable = true,
            ShowPercentage = true,
            InitialProgress = 0
        };

        var progress = await _messageBoxService.ShowProgressAsync(options);

        try
        {
            var files = new[] { "Document.pdf", "Image.png", "Data.xlsx", "Report.docx", "Archive.zip" };

            for (int i = 0; i < files.Length; i++)
            {
                if (progress.IsCancellationRequested)
                {
                    LastResult = "Progress dialog: Download cancelled";
                    return;
                }

                progress.SetMessage($"Downloading {files[i]}...");
                progress.SetProgress((i + 1) * 20);

                // Simulate download time
                await Task.Delay(800, progress.CancellationToken);
            }

            LastResult = $"Progress dialog: Downloaded {files.Length} files successfully";
        }
        catch (OperationCanceledException)
        {
            LastResult = "Progress dialog: Download cancelled";
        }
        finally
        {
            await progress.CloseAsync();
        }
    }

    /// <summary>
    /// Demonstrates RunWithProgressAsync for automatic progress management.
    /// </summary>
    /// <remarks>
    /// This is the simplest way to show progress for a long-running task.
    /// The dialog opens automatically and closes when the task completes.
    /// </remarks>
    [RelayCommand]
    private async Task ShowProgressAutoAsync()
    {
        var options = new ProgressOptions
        {
            Title = "Analyzing",
            Message = "Running analysis...",
            IsIndeterminate = false,
            IsCancellable = true,
            ShowPercentage = true
        };

        try
        {
            // RunWithProgressAsync handles the dialog lifecycle automatically
            // The operation receives IProgress<double> for reporting (0-100) and CancellationToken
            var result = await _messageBoxService.RunWithProgressAsync(
                async (progress, cancellationToken) =>
                {
                    // Simulate complex analysis with progress reporting
                    for (int i = 0; i <= 100; i += 10)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        progress.Report(i);
                        await Task.Delay(300, cancellationToken);
                    }

                    // Return a result from the operation
                    return "Analysis completed: 42 items processed";
                },
                options
            );

            LastResult = $"Progress dialog: {result ?? "Cancelled"}";
        }
        catch (OperationCanceledException)
        {
            LastResult = "Progress dialog: Cancelled by user";
        }
        catch (Exception ex)
        {
            LastResult = $"Progress dialog: Error - {ex.Message}";
        }
    }

    #endregion

    #region Toast Notification Commands

    /// <summary>
    /// Shows an info toast notification.
    /// </summary>
    /// <remarks>
    /// Toast notifications are non-blocking and don't require user interaction.
    /// They automatically disappear after a configurable duration.
    /// </remarks>
    [RelayCommand]
    private void ShowToastInfo()
    {
        _toastService.ShowInfo("This is an informational toast notification.");
        LastResult = "Toast: Info notification shown";
    }

    /// <summary>
    /// Shows a success toast notification.
    /// </summary>
    [RelayCommand]
    private void ShowToastSuccess()
    {
        _toastService.ShowSuccess("Operation completed successfully!");
        LastResult = "Toast: Success notification shown";
    }

    /// <summary>
    /// Shows a warning toast notification.
    /// </summary>
    [RelayCommand]
    private void ShowToastWarning()
    {
        _toastService.ShowWarning("Please review your settings before proceeding.");
        LastResult = "Toast: Warning notification shown";
    }

    /// <summary>
    /// Shows an error toast notification.
    /// </summary>
    [RelayCommand]
    private void ShowToastError()
    {
        _toastService.ShowError("Failed to save changes. Please try again.");
        LastResult = "Toast: Error notification shown";
    }

    /// <summary>
    /// Shows a custom toast with advanced options.
    /// </summary>
    /// <remarks>
    /// Demonstrates full customization including title, duration,
    /// click handler, and close callback.
    /// </remarks>
    [RelayCommand]
    private void ShowToastCustom()
    {
        var options = new ToastOptions
        {
            Title = "File Saved",
            Message = "Document.pdf has been saved. Click to open.",
            Type = ToastType.Success,
            Duration = TimeSpan.FromSeconds(8),
            IsClickToClose = true,
            OnClick = () => Debug.WriteLine("Toast clicked - would open file"),
            OnClose = () => Debug.WriteLine("Toast closed")
        };

        _toastService.Show(options);
        LastResult = "Toast: Custom notification shown (click to dismiss, 8s duration)";
    }

    /// <summary>
    /// Shows multiple toasts to demonstrate stacking behavior.
    /// </summary>
    [RelayCommand]
    private void ShowToastMultiple()
    {
        _toastService.ShowInfo("First notification");
        _toastService.ShowSuccess("Second notification");
        _toastService.ShowWarning("Third notification");
        _toastService.ShowError("Fourth notification");
        LastResult = "Toast: 4 stacked notifications shown";
    }

    /// <summary>
    /// Closes all active toast notifications.
    /// </summary>
    [RelayCommand]
    private void CloseAllToasts()
    {
        _toastService.CloseAll();
        LastResult = "Toast: All notifications closed";
    }

    #endregion

    #region Fluent Builder Commands

    /// <summary>
    /// Shows a simple dialog using the fluent builder API.
    /// </summary>
    /// <remarks>
    /// The fluent builder provides a readable, chainable API for
    /// constructing dialogs without creating MessageBoxOptions manually.
    /// </remarks>
    [RelayCommand]
    private async Task ShowBuilderSimpleAsync()
    {
        // Using the fluent builder with extension method shorthand
        var result = await _messageBoxService
            .Info("This dialog was created using the fluent builder API!", "Welcome")
            .ShowAsync();

        LastResult = $"Builder (simple): {result.Result}";
    }

    /// <summary>
    /// Shows a confirmation dialog using the builder with callbacks.
    /// </summary>
    /// <remarks>
    /// Callbacks allow you to execute code when specific buttons are clicked,
    /// providing a cleaner alternative to switch statements on the result.
    /// </remarks>
    [RelayCommand]
    private async Task ShowBuilderWithCallbacksAsync()
    {
        string actionTaken = "None";

        // Using MessageBoxBuilder.Create() with callbacks
        await MessageBoxBuilder
            .Create(_messageBoxService)
            .WithTitle("Save Document")
            .WithMessage("Do you want to save changes before closing?")
            .WithIcon(MessageBoxIcon.Question)
            .WithButtons(MessageBoxButtons.YesNoCancel)
            .OnYes(() => actionTaken = "Saved")
            .OnNo(() => actionTaken = "Discarded")
            .OnCancel(() => actionTaken = "Cancelled")
            .ShowAsync();

        LastResult = $"Builder (callbacks): Action = {actionTaken}";
    }

    /// <summary>
    /// Shows a dialog using the static builder shorthand.
    /// </summary>
    /// <remarks>
    /// MessageBoxBuilder.Create() provides a static entry point
    /// when dependency injection is not available or preferred.
    /// </remarks>
    [RelayCommand]
    private async Task ShowBuilderStaticAsync()
    {
        var result = await MessageBoxBuilder
            .Create(_messageBoxService)
            .WithTitle("Quick Question")
            .WithMessage("Are you enjoying the FluentDialogs library?")
            .WithIcon(MessageBoxIcon.Question)
            .WithButtons(MessageBoxButtons.YesNo)
            .OnYes(() => _toastService.ShowSuccess("Thank you! 🎉"))
            .OnNo(() => _toastService.ShowInfo("We'd love to hear your feedback!"))
            .ShowAsync();

        LastResult = $"Builder (static): {result}";
    }

    #endregion

    #region Type Alias Demo Commands

    /// <summary>
    /// Demonstrates using the FluentMessageBoxOptions type alias.
    /// </summary>
    /// <remarks>
    /// <para>
    /// FluentMessageBoxOptions is a type alias for MessageBoxOptions that
    /// provides a distinct type name to avoid conflicts with other libraries
    /// that define MessageBoxOptions (e.g., System.Windows.Forms).
    /// </para>
    /// <para>
    /// Both types are functionally identical and interchangeable.
    /// </para>
    /// </remarks>
    [RelayCommand]
    private async Task ShowFluentAliasAsync()
    {
        // Using the Fluent-prefixed alias instead of MessageBoxOptions
        // This avoids naming conflicts with System.Windows.Forms.MessageBoxOptions
        var options = new FluentMessageBoxOptions
        {
            Title = "Type Alias Demo",
            Message = "This dialog uses FluentMessageBoxOptions - a type alias that avoids namespace conflicts with System.Windows.Forms.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK
        };

        // ShowExtendedAsync returns DialogResult, which is the base of FluentDialogResult
        var result = await _messageBoxService.ShowExtendedAsync(options);

        LastResult = $"FluentMessageBoxOptions: Result={result.Result}";
    }

    #endregion
}