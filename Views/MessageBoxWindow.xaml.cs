using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FluentDialogs.ViewModels;

namespace FluentDialogs.Views;

/// <summary>
/// Code-behind for the MessageBoxWindow.
/// Handles window chrome interactions and event routing from ViewModel.
/// </summary>
public partial class MessageBoxWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxWindow"/> class.
    /// </summary>
    public MessageBoxWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
    }

    /// <summary>
    /// Gets or sets the result of the message box dialog.
    /// </summary>
    public MessageBoxResult Result { get; set; } = MessageBoxResult.None;

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (DetailedTextScroller != null)
        {
            DetailedTextScroller.ScrollChanged += OnDetailedTextScrollChanged;
        }

        if (PasswordBox != null && DataContext is MessageBoxViewModel vm && vm.InputIsPassword)
        {
            PasswordBox.PasswordChanged += OnPasswordChanged;
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is MessageBoxViewModel oldViewModel)
        {
            oldViewModel.CloseRequested -= OnCloseRequested;
        }

        if (e.NewValue is MessageBoxViewModel newViewModel)
        {
            newViewModel.CloseRequested += OnCloseRequested;
        }
    }

    private void OnCloseRequested(object? sender, MessageBoxResult result)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.StopTimeoutTimer();

            if (PasswordBox != null && viewModel.InputIsPassword)
            {
                viewModel.InputText = PasswordBox.Password;
            }
        }

        Result = result;
        DialogResult = result != MessageBoxResult.None;
        Close();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // DragMove can throw if window state changes during drag
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.StopTimeoutTimer();
        }

        Result = MessageBoxResult.Cancel;
        DialogResult = false;
        Close();
    }

    private void OnDetailedTextScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer && DataContext is MessageBoxViewModel viewModel)
        {
            var isAtBottom = scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 10;
            if (isAtBottom && viewModel.RequireScrollToBottom)
            {
                viewModel.HasScrolledToBottom = true;
            }
        }
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox && DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.InputText = passwordBox.Password;
        }
    }

    /// <inheritdoc/>
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.CloseRequested -= OnCloseRequested;
            viewModel.StopTimeoutTimer();
        }

        if (DetailedTextScroller != null)
        {
            DetailedTextScroller.ScrollChanged -= OnDetailedTextScrollChanged;
        }

        if (PasswordBox != null)
        {
            PasswordBox.PasswordChanged -= OnPasswordChanged;
        }

        DataContextChanged -= OnDataContextChanged;
        Loaded -= OnWindowLoaded;
        base.OnClosed(e);
    }
}
