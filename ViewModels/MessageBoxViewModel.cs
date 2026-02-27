using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FluentDialogs.Enums;
using FluentDialogs.Models;
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;

namespace FluentDialogs.ViewModels;

/// <summary>
/// ViewModel for the message box dialog window.
/// </summary>
public sealed class MessageBoxViewModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _message = string.Empty;
    private MessageBoxIcon _icon = MessageBoxIcon.None;
    private object? _content;
    private Exception? _exception;
    private bool _isStackTraceVisible;
    private MessageBoxResult _result = MessageBoxResult.None;

    // Sizing and appearance fields
    private double? _dialogWidth;
    private double? _dialogHeight;
    private double? _dialogMinWidth;
    private double? _dialogMinHeight;
    private double? _dialogMaxWidth;
    private double? _dialogMaxHeight;
    private System.Windows.Media.Color? _titleBarColor;

    // Native feature fields
    private string? _checkboxText;
    private bool _isCheckboxChecked;
    private string? _hyperlinkText;
    private string? _hyperlinkUrl;
    private string? _inputText;
    private string? _inputPlaceholder;
    private bool _inputIsPassword;
    private ObservableCollection<object>? _selectionItems;
    private object? _selectedItem;
    private int _selectedIndex = -1;
    private string? _selectionDisplayMemberPath;
    private ObservableCollection<object>? _dropdownItems;
    private object? _dropdownSelectedItem;
    private int _dropdownSelectedIndex = -1;
    private string? _dropdownDisplayMemberPath;
    private bool _isResizable;
    private int _timeoutSeconds;
    private int _remainingSeconds;
    private string? _detailedText;
    private bool _requireScrollToBottom;
    private bool _hasScrolledToBottom;
    private DispatcherTimer? _timeoutTimer;
    private MessageBoxResult _timeoutResult = MessageBoxResult.Cancel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxViewModel"/> class.
    /// </summary>
    public MessageBoxViewModel()
    {
        Buttons = new ObservableCollection<MessageBoxButtonViewModel>();
        ButtonClickCommand = new RelayCommand<MessageBoxButtonViewModel>(OnButtonClick);
        ToggleStackTraceCommand = new RelayCommand(OnToggleStackTrace);
        CopyExceptionCommand = new RelayCommand(OnCopyException);
        CopyMessageCommand = new RelayCommand(OnCopyMessage);
        OpenHyperlinkCommand = new RelayCommand(OnOpenHyperlink);
        DetailedTextScrolledCommand = new RelayCommand(OnDetailedTextScrolled);
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when the dialog should be closed.
    /// </summary>
    public event EventHandler<MessageBoxResult>? CloseRequested;

    /// <summary>
    /// Gets or sets the title text displayed in the dialog.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the main message text displayed in the dialog.
    /// </summary>
    public string Message
    {
        get => _message;
        set
        {
            if (_message != value)
            {
                _message = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the icon displayed in the dialog.
    /// </summary>
    public MessageBoxIcon Icon
    {
        get => _icon;
        set
        {
            if (_icon != value)
            {
                _icon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsErrorDialog));
                OnPropertyChanged(nameof(IsWarningDialog));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is an error dialog.
    /// </summary>
    public bool IsErrorDialog => _icon == MessageBoxIcon.Error;

    /// <summary>
    /// Gets a value indicating whether this is a warning dialog.
    /// </summary>
    public bool IsWarningDialog => _icon == MessageBoxIcon.Warning;

    /// <summary>
    /// Gets or sets optional custom content to display below the message.
    /// </summary>
    public object? Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasContent));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether custom content is present.
    /// </summary>
    public bool HasContent => _content != null;

    /// <summary>
    /// Gets or sets the exception to display.
    /// </summary>
    public Exception? Exception
    {
        get => _exception;
        set
        {
            if (_exception != value)
            {
                _exception = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasException));
                OnPropertyChanged(nameof(ExceptionMessage));
                OnPropertyChanged(nameof(ExceptionStackTrace));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether an exception is present.
    /// </summary>
    public bool HasException => _exception != null;

    /// <summary>
    /// Gets the exception message.
    /// </summary>
    public string ExceptionMessage => _exception?.Message ?? string.Empty;

    /// <summary>
    /// Gets the exception stack trace.
    /// </summary>
    public string ExceptionStackTrace => _exception?.StackTrace ?? string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the stack trace is visible.
    /// </summary>
    public bool IsStackTraceVisible
    {
        get => _isStackTraceVisible;
        set
        {
            if (_isStackTraceVisible != value)
            {
                _isStackTraceVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StackTraceToggleText));
            }
        }
    }

    /// <summary>
    /// Gets the text for the stack trace toggle button.
    /// </summary>
    public string StackTraceToggleText => _isStackTraceVisible ? "Hide Details" : "Show Details";

    /// <summary>
    /// Gets the collection of button view models.
    /// </summary>
    public ObservableCollection<MessageBoxButtonViewModel> Buttons { get; }

    /// <summary>
    /// Gets the result of the dialog.
    /// </summary>
    public MessageBoxResult Result
    {
        get => _result;
        private set
        {
            if (_result != value)
            {
                _result = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the command executed when a button is clicked.
    /// </summary>
    public ICommand ButtonClickCommand { get; }

    /// <summary>
    /// Gets the command to toggle stack trace visibility.
    /// </summary>
    public ICommand ToggleStackTraceCommand { get; }

    /// <summary>
    /// Gets the command to copy exception details to clipboard.
    /// </summary>
    public ICommand CopyExceptionCommand { get; }

    /// <summary>
    /// Gets the command to copy the message to clipboard.
    /// </summary>
    public ICommand CopyMessageCommand { get; }

    /// <summary>
    /// Gets the command to open hyperlink in browser.
    /// </summary>
    public ICommand OpenHyperlinkCommand { get; }

    /// <summary>
    /// Gets the command executed when detailed text is scrolled to bottom.
    /// </summary>
    public ICommand DetailedTextScrolledCommand { get; }

    #region Native Feature Properties

    /// <summary>
    /// Gets or sets the checkbox text.
    /// </summary>
    public string? CheckboxText
    {
        get => _checkboxText;
        set
        {
            if (_checkboxText != value)
            {
                _checkboxText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCheckbox));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether a checkbox should be shown.
    /// </summary>
    public bool HasCheckbox => !string.IsNullOrEmpty(_checkboxText);

    /// <summary>
    /// Gets or sets whether the checkbox is checked.
    /// </summary>
    public bool IsCheckboxChecked
    {
        get => _isCheckboxChecked;
        set
        {
            if (_isCheckboxChecked != value)
            {
                _isCheckboxChecked = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the hyperlink text.
    /// </summary>
    public string? HyperlinkText
    {
        get => _hyperlinkText;
        set
        {
            if (_hyperlinkText != value)
            {
                _hyperlinkText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasHyperlink));
            }
        }
    }

    /// <summary>
    /// Gets or sets the hyperlink URL.
    /// </summary>
    public string? HyperlinkUrl
    {
        get => _hyperlinkUrl;
        set
        {
            if (_hyperlinkUrl != value)
            {
                _hyperlinkUrl = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether a hyperlink should be shown.
    /// </summary>
    public bool HasHyperlink => !string.IsNullOrEmpty(_hyperlinkText) && !string.IsNullOrEmpty(_hyperlinkUrl);

    /// <summary>
    /// Gets or sets the input text.
    /// </summary>
    public string? InputText
    {
        get => _inputText;
        set
        {
            if (_inputText != value)
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the input placeholder text.
    /// </summary>
    public string? InputPlaceholder
    {
        get => _inputPlaceholder;
        set
        {
            if (_inputPlaceholder != value)
            {
                _inputPlaceholder = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasInput));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether an input field should be shown.
    /// </summary>
    public bool HasInput => _inputPlaceholder != null;

    /// <summary>
    /// Gets or sets whether the input is password mode.
    /// </summary>
    public bool InputIsPassword
    {
        get => _inputIsPassword;
        set
        {
            if (_inputIsPassword != value)
            {
                _inputIsPassword = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selection items.
    /// </summary>
    public ObservableCollection<object>? SelectionItems
    {
        get => _selectionItems;
        set
        {
            if (_selectionItems != value)
            {
                _selectionItems = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether a selection list should be shown.
    /// </summary>
    public bool HasSelection => _selectionItems is { Count: > 0 };

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public object? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index.
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex != value)
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the display member path for selection items.
    /// </summary>
    public string? SelectionDisplayMemberPath
    {
        get => _selectionDisplayMemberPath;
        set
        {
            if (_selectionDisplayMemberPath != value)
            {
                _selectionDisplayMemberPath = value;
                OnPropertyChanged();
            }
        }
    }

    // ═══════════════ Dropdown Properties ═══════════════

    /// <summary>
    /// Gets or sets the dropdown items.
    /// </summary>
    public ObservableCollection<object>? DropdownItems
    {
        get => _dropdownItems;
        set
        {
            if (_dropdownItems != value)
            {
                _dropdownItems = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDropdown));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether a dropdown should be shown.
    /// </summary>
    public bool HasDropdown => _dropdownItems is { Count: > 0 };

    /// <summary>
    /// Gets or sets the dropdown selected item.
    /// </summary>
    public object? DropdownSelectedItem
    {
        get => _dropdownSelectedItem;
        set
        {
            if (_dropdownSelectedItem != value)
            {
                _dropdownSelectedItem = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the dropdown selected index.
    /// </summary>
    public int DropdownSelectedIndex
    {
        get => _dropdownSelectedIndex;
        set
        {
            if (_dropdownSelectedIndex != value)
            {
                _dropdownSelectedIndex = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the display member path for dropdown items.
    /// </summary>
    public string? DropdownDisplayMemberPath
    {
        get => _dropdownDisplayMemberPath;
        set
        {
            if (_dropdownDisplayMemberPath != value)
            {
                _dropdownDisplayMemberPath = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is resizable.
    /// </summary>
    public bool IsResizable
    {
        get => _isResizable;
        set
        {
            if (_isResizable != value)
            {
                _isResizable = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the timeout in seconds.
    /// </summary>
    public int TimeoutSeconds
    {
        get => _timeoutSeconds;
        set
        {
            if (_timeoutSeconds != value)
            {
                _timeoutSeconds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTimeout));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the dialog has a timeout.
    /// </summary>
    public bool HasTimeout => _timeoutSeconds > 0;

    /// <summary>
    /// Gets or sets the remaining seconds before timeout.
    /// </summary>
    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set
        {
            if (_remainingSeconds != value)
            {
                _remainingSeconds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeoutDisplayText));
            }
        }
    }

    /// <summary>
    /// Gets the timeout display text.
    /// </summary>
    public string TimeoutDisplayText => HasTimeout ? $"Closing in {_remainingSeconds}s..." : string.Empty;

    /// <summary>
    /// Gets or sets the detailed text content.
    /// </summary>
    public string? DetailedText
    {
        get => _detailedText;
        set
        {
            if (_detailedText != value)
            {
                _detailedText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDetailedText));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether detailed text should be shown.
    /// </summary>
    public bool HasDetailedText => !string.IsNullOrEmpty(_detailedText);

    /// <summary>
    /// Gets or sets whether scroll to bottom is required.
    /// </summary>
    public bool RequireScrollToBottom
    {
        get => _requireScrollToBottom;
        set
        {
            if (_requireScrollToBottom != value)
            {
                _requireScrollToBottom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAccept));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether user has scrolled to bottom.
    /// </summary>
    public bool HasScrolledToBottom
    {
        get => _hasScrolledToBottom;
        set
        {
            if (_hasScrolledToBottom != value)
            {
                _hasScrolledToBottom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAccept));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the accept button should be enabled.
    /// </summary>
    public bool CanAccept => !_requireScrollToBottom || _hasScrolledToBottom;

    /// <summary>
    /// Gets a value indicating whether the dialog timed out.
    /// </summary>
    public bool TimedOut { get; private set; }

    #endregion

    #region Dialog Sizing and Appearance Properties

    /// <summary>
    /// Gets the preferred width of the dialog.
    /// </summary>
    public double? DialogWidth
    {
        get => _dialogWidth;
        set
        {
            if (_dialogWidth != value)
            {
                _dialogWidth = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the preferred height of the dialog.
    /// </summary>
    public double? DialogHeight
    {
        get => _dialogHeight;
        set
        {
            if (_dialogHeight != value)
            {
                _dialogHeight = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the minimum width of the dialog.
    /// </summary>
    public double DialogMinWidth
    {
        get => _dialogMinWidth ?? 320;
        set
        {
            if (_dialogMinWidth != value)
            {
                _dialogMinWidth = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the minimum height of the dialog.
    /// </summary>
    public double DialogMinHeight
    {
        get => _dialogMinHeight ?? 150;
        set
        {
            if (_dialogMinHeight != value)
            {
                _dialogMinHeight = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the maximum width of the dialog.
    /// </summary>
    public double DialogMaxWidth
    {
        get => _dialogMaxWidth ?? 800;
        set
        {
            if (_dialogMaxWidth != value)
            {
                _dialogMaxWidth = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the maximum height of the dialog.
    /// </summary>
    public double DialogMaxHeight
    {
        get => _dialogMaxHeight ?? 600;
        set
        {
            if (_dialogMaxHeight != value)
            {
                _dialogMaxHeight = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the custom title bar color.
    /// </summary>
    public System.Windows.Media.Color? TitleBarColor
    {
        get => _titleBarColor;
        set
        {
            if (_titleBarColor != value)
            {
                _titleBarColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCustomTitleBarColor));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether a custom title bar color is set.
    /// </summary>
    public bool HasCustomTitleBarColor => _titleBarColor.HasValue;

    #endregion

    /// <summary>
    /// Gets the full exception text for copying.
    /// </summary>
    public string FullExceptionText
    {
        get
        {
            if (_exception == null)
            {
                return string.Empty;
            }

            return $"Exception: {_exception.GetType().FullName}\n" +
                   $"Message: {_exception.Message}\n" +
                   $"Stack Trace:\n{_exception.StackTrace}" +
                   (_exception.InnerException != null ? $"\n\nInner Exception: {_exception.InnerException}" : string.Empty);
        }
    }

    /// <summary>
    /// Initializes the view model with the specified options.
    /// </summary>
    /// <param name="options">The message box options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public void Initialize(MessageBoxOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.Validate();

        Title = options.Title;
        Message = options.Message;
        Icon = options.Icon;
        Content = options.Content;
        Exception = options.Exception;

        // Initialize native feature properties
        CheckboxText = options.CheckboxText;
        IsCheckboxChecked = options.CheckboxChecked;
        HyperlinkText = options.HyperlinkText;
        HyperlinkUrl = options.HyperlinkUrl;
        InputPlaceholder = options.InputPlaceholder;
        InputText = options.InputDefaultValue ?? string.Empty;
        InputIsPassword = options.InputIsPassword;
        SelectionDisplayMemberPath = options.SelectionDisplayMemberPath;
        DropdownDisplayMemberPath = options.DropdownDisplayMemberPath;
        IsResizable = options.IsResizable;
        DetailedText = options.DetailedText;
        RequireScrollToBottom = options.RequireScrollToBottom;
        HasScrolledToBottom = !options.RequireScrollToBottom;
        TimeoutSeconds = options.TimeoutSeconds;
        RemainingSeconds = options.TimeoutSeconds;
        _timeoutResult = options.TimeoutResult;

        // Initialize sizing and appearance properties
        DialogWidth = options.Width;
        DialogHeight = options.Height;
        if (options.MinWidth.HasValue) DialogMinWidth = options.MinWidth.Value;
        if (options.MinHeight.HasValue) DialogMinHeight = options.MinHeight.Value;
        if (options.MaxWidth.HasValue) DialogMaxWidth = options.MaxWidth.Value;
        if (options.MaxHeight.HasValue) DialogMaxHeight = options.MaxHeight.Value;
        TitleBarColor = options.TitleBarColor;

        if (options.SelectionItems is { Count: > 0 })
        {
            SelectionItems = new ObservableCollection<object>(options.SelectionItems);
            if (options.SelectionDefaultIndex >= 0 && options.SelectionDefaultIndex < options.SelectionItems.Count)
            {
                SelectedIndex = options.SelectionDefaultIndex;
                SelectedItem = options.SelectionItems[options.SelectionDefaultIndex];
            }
        }

        if (options.DropdownItems is { Count: > 0 })
        {
            DropdownItems = new ObservableCollection<object>(options.DropdownItems);
            if (options.DropdownDefaultIndex >= 0 && options.DropdownDefaultIndex < options.DropdownItems.Count)
            {
                DropdownSelectedIndex = options.DropdownDefaultIndex;
                DropdownSelectedItem = options.DropdownItems[options.DropdownDefaultIndex];
            }
        }

        Buttons.Clear();

        if (options.CustomButtons is { Count: > 0 })
        {
            foreach (var buttonDef in options.CustomButtons)
            {
                Buttons.Add(new MessageBoxButtonViewModel(buttonDef, ButtonClickCommand));
            }
        }
        else if (options.Buttons.HasValue)
        {
            GenerateStandardButtons(options.Buttons.Value);
        }

        // Start timeout timer if configured
        if (TimeoutSeconds > 0)
        {
            StartTimeoutTimer();
        }
    }

    /// <summary>
    /// Stops the timeout timer and cleans up.
    /// </summary>
    public void StopTimeoutTimer()
    {
        if (_timeoutTimer != null)
        {
            _timeoutTimer.Stop();
            _timeoutTimer = null;
        }
    }

    private void StartTimeoutTimer()
    {
        _timeoutTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timeoutTimer.Tick += OnTimeoutTick;
        _timeoutTimer.Start();
    }

    private void OnTimeoutTick(object? sender, EventArgs e)
    {
        RemainingSeconds--;
        if (RemainingSeconds <= 0)
        {
            StopTimeoutTimer();
            TimedOut = true;
            Result = _timeoutResult;
            CloseRequested?.Invoke(this, _timeoutResult);
        }
    }

    private void GenerateStandardButtons(MessageBoxButtons buttons)
    {
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                AddButton("OK", MessageBoxResult.OK, ButtonStyle.Primary, isDefault: true);
                break;

            case MessageBoxButtons.OKCancel:
                AddButton("OK", MessageBoxResult.OK, ButtonStyle.Primary, isDefault: true);
                AddButton("Cancel", MessageBoxResult.Cancel, ButtonStyle.Default, isCancel: true);
                break;

            case MessageBoxButtons.YesNo:
                AddButton("Yes", MessageBoxResult.Yes, ButtonStyle.Primary, isDefault: true);
                AddButton("No", MessageBoxResult.No, ButtonStyle.Default, isCancel: true);
                break;

            case MessageBoxButtons.YesNoCancel:
                AddButton("Yes", MessageBoxResult.Yes, ButtonStyle.Primary, isDefault: true);
                AddButton("No", MessageBoxResult.No, ButtonStyle.Default);
                AddButton("Cancel", MessageBoxResult.Cancel, ButtonStyle.Default, isCancel: true);
                break;

            case MessageBoxButtons.RetryCancel:
                AddButton("Retry", MessageBoxResult.Yes, ButtonStyle.Primary, isDefault: true);
                AddButton("Cancel", MessageBoxResult.Cancel, ButtonStyle.Default, isCancel: true);
                break;

            case MessageBoxButtons.AbortRetryIgnore:
                AddButton("Abort", MessageBoxResult.Cancel, ButtonStyle.Danger);
                AddButton("Retry", MessageBoxResult.Yes, ButtonStyle.Primary, isDefault: true);
                AddButton("Ignore", MessageBoxResult.No, ButtonStyle.Default);
                break;
        }
    }

    private void AddButton(string text, MessageBoxResult result, ButtonStyle style, bool isDefault = false, bool isCancel = false)
    {
        var definition = new MessageBoxButtonDefinition
        {
            Text = text,
            Result = result,
            Style = style,
            IsDefault = isDefault,
            IsCancel = isCancel
        };

        Buttons.Add(new MessageBoxButtonViewModel(definition, ButtonClickCommand));
    }

    private void OnButtonClick(MessageBoxButtonViewModel? buttonViewModel)
    {
        if (buttonViewModel == null)
        {
            return;
        }

        if (buttonViewModel.CustomCommand?.CanExecute(buttonViewModel.CustomCommandParameter) == true)
        {
            buttonViewModel.CustomCommand.Execute(buttonViewModel.CustomCommandParameter);
        }

        Result = buttonViewModel.Result;
        CloseRequested?.Invoke(this, buttonViewModel.Result);
    }

    private void OnToggleStackTrace()
    {
        IsStackTraceVisible = !IsStackTraceVisible;
    }

    private void OnCopyException()
    {
        try
        {
            Clipboard.SetText(FullExceptionText);
        }
        catch
        {
            // Clipboard operations can fail - ignore silently
        }
    }

    private void OnCopyMessage()
    {
        try
        {
            var text = $"{Title}\n\n{Message}";
            if (HasException)
            {
                text += $"\n\n{FullExceptionText}";
            }
            Clipboard.SetText(text);
        }
        catch
        {
            // Clipboard operations can fail - ignore silently
        }
    }

    private void OnOpenHyperlink()
    {
        if (string.IsNullOrEmpty(_hyperlinkUrl))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _hyperlinkUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            // Failed to open URL - ignore silently
        }
    }

    private void OnDetailedTextScrolled()
    {
        HasScrolledToBottom = true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }

    private class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;

        public RelayCommand(Action<T?> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute(parameter is T typedParameter ? typedParameter : default);
    }
}
