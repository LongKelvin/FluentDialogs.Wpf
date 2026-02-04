using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FluentDialogs.Enums;
using FluentDialogs.Models;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxViewModel"/> class.
    /// </summary>
    public MessageBoxViewModel()
    {
        Buttons = new ObservableCollection<MessageBoxButtonViewModel>();
        ButtonClickCommand = new RelayCommand<MessageBoxButtonViewModel>(OnButtonClick);
        ToggleStackTraceCommand = new RelayCommand(OnToggleStackTrace);
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when the dialog should be closed.
    /// </summary>
    public event EventHandler? CloseRequested;

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
            }
        }
    }

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
    /// Initializes the view model with the specified options.
    /// </summary>
    /// <param name="options">The message box options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public void Initialize(Models.MessageBoxOptions options)
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
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnToggleStackTrace()
    {
        IsStackTraceVisible = !IsStackTraceVisible;
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
