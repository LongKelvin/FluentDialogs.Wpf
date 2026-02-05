using System.ComponentModel;
using System.Windows;
using FluentDialogs.Models;

namespace FluentDialogs.Views;

/// <summary>
/// Progress dialog window that displays operation progress with optional cancellation.
/// </summary>
public partial class ProgressWindow : Window, IProgressController, INotifyPropertyChanged
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private string _message = string.Empty;
    private double _progress;
    private bool _isIndeterminate = true;
    private bool _showPercentage = true;
    private bool _isCancellable = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressWindow"/> class.
    /// </summary>
    public ProgressWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the message displayed in the dialog.
    /// </summary>
    public string Message
    {
        get => _message;
        set
        {
            if (_message != value)
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
    }

    /// <summary>
    /// Gets or sets the current progress value (0-100).
    /// </summary>
    public double Progress
    {
        get => _progress;
        set
        {
            if (Math.Abs(_progress - value) > 0.01)
            {
                _progress = Math.Clamp(value, 0, 100);
                OnPropertyChanged(nameof(Progress));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the progress is indeterminate.
    /// </summary>
    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        set
        {
            if (_isIndeterminate != value)
            {
                _isIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
                OnPropertyChanged(nameof(PercentageVisibility));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the percentage text.
    /// </summary>
    public bool ShowPercentage
    {
        get => _showPercentage;
        set
        {
            if (_showPercentage != value)
            {
                _showPercentage = value;
                OnPropertyChanged(nameof(ShowPercentage));
                OnPropertyChanged(nameof(PercentageVisibility));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the operation can be cancelled.
    /// </summary>
    public bool IsCancellable
    {
        get => _isCancellable;
        set
        {
            if (_isCancellable != value)
            {
                _isCancellable = value;
                OnPropertyChanged(nameof(IsCancellable));
                OnPropertyChanged(nameof(CancelButtonVisibility));
            }
        }
    }

    /// <summary>
    /// Gets the visibility of the percentage text.
    /// </summary>
    public Visibility PercentageVisibility =>
        !IsIndeterminate && ShowPercentage ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility of the cancel button.
    /// </summary>
    public Visibility CancelButtonVisibility =>
        IsCancellable ? Visibility.Visible : Visibility.Collapsed;

    /// <inheritdoc/>
    public bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;

    /// <inheritdoc/>
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    /// <inheritdoc/>
    void IProgressController.SetProgress(double value)
    {
        if (Dispatcher.CheckAccess())
        {
            Progress = value;
        }
        else
        {
            Dispatcher.Invoke(() => Progress = value);
        }
    }

    /// <inheritdoc/>
    void IProgressController.SetMessage(string message)
    {
        if (Dispatcher.CheckAccess())
        {
            Message = message;
        }
        else
        {
            Dispatcher.Invoke(() => Message = message);
        }
    }

    /// <inheritdoc/>
    void IProgressController.SetIndeterminate(bool isIndeterminate)
    {
        if (Dispatcher.CheckAccess())
        {
            IsIndeterminate = isIndeterminate;
        }
        else
        {
            Dispatcher.Invoke(() => IsIndeterminate = isIndeterminate);
        }
    }

    /// <inheritdoc/>
    Task IProgressController.CloseAsync()
    {
        if (Dispatcher.CheckAccess())
        {
            Close();
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource();
        Dispatcher.Invoke(() =>
        {
            Close();
            tcs.SetResult();
        });
        return tcs.Task;
    }

    /// <summary>
    /// Initializes the window with the specified options.
    /// </summary>
    /// <param name="options">The progress options.</param>
    internal void Initialize(ProgressOptions options)
    {
        Title = options.Title;
        Message = options.Message;
        IsIndeterminate = options.IsIndeterminate;
        IsCancellable = options.IsCancellable;
        ShowPercentage = options.ShowPercentage;
        Progress = options.InitialProgress;

        if (options.Width.HasValue)
        {
            Width = options.Width.Value;
            SizeToContent = SizeToContent.Height;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        CancelButton.IsEnabled = false;
        CancelButton.Content = "Cancelling...";
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc/>
    protected override void OnClosing(CancelEventArgs e)
    {
        _cancellationTokenSource.Dispose();
        base.OnClosing(e);
    }
}
