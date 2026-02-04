using System.Windows;
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
    }

    /// <summary>
    /// Gets or sets the result of the message box dialog.
    /// </summary>
    public MessageBoxResult Result { get; set; } = MessageBoxResult.None;

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
        Result = result;
        DialogResult = result != MessageBoxResult.None;
        Close();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Result = MessageBoxResult.Cancel;
        DialogResult = false;
        Close();
    }

    /// <inheritdoc/>
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.CloseRequested -= OnCloseRequested;
        }

        DataContextChanged -= OnDataContextChanged;
        base.OnClosed(e);
    }
}
