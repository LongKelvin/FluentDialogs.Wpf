using System.Windows;
using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.Views;

/// <summary>
/// Container window for displaying toast notifications.
/// </summary>
public partial class ToastContainerWindow : Window
{
    private ToastPosition _position = ToastPosition.TopRight;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastContainerWindow"/> class.
    /// </summary>
    public ToastContainerWindow()
    {
        InitializeComponent();
        UpdatePosition();
    }

    /// <summary>
    /// Gets the current number of toasts displayed.
    /// </summary>
    public int ToastCount => ToastContainer.Items.Count;

    /// <summary>
    /// Gets or sets the position of the toast container.
    /// </summary>
    public ToastPosition Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdatePosition();
        }
    }

    /// <summary>
    /// Adds a toast notification to the container.
    /// </summary>
    /// <param name="options">The toast options.</param>
    /// <param name="maxToasts">The maximum number of toasts allowed.</param>
    public void AddToast(ToastOptions options, int maxToasts)
    {
        // Respect position from options
        if (options.Position != _position)
        {
            Position = options.Position;
        }

        // Remove oldest toasts if limit exceeded
        while (ToastContainer.Items.Count >= maxToasts && ToastContainer.Items.Count > 0)
        {
            if (ToastContainer.Items[0] is ToastControl oldestToast)
            {
                oldestToast.Close();
            }
        }

        var toast = new ToastControl(options);
        toast.Closing += (_, _) =>
        {
            ToastContainer.Items.Remove(toast);
            if (ToastContainer.Items.Count == 0)
            {
                Hide();
            }
        };

        ToastContainer.Items.Add(toast);

        if (!IsVisible)
        {
            Show();
        }

        UpdatePosition();
    }

    /// <summary>
    /// Closes all displayed toasts.
    /// </summary>
    public void CloseAllToasts()
    {
        var toasts = ToastContainer.Items.Cast<ToastControl>().ToList();
        foreach (var toast in toasts)
        {
            toast.Close();
        }
    }

    private void UpdatePosition()
    {
        var workArea = SystemParameters.WorkArea;

        switch (_position)
        {
            case ToastPosition.TopRight:
                Left = workArea.Right - ActualWidth - 16;
                Top = workArea.Top + 16;
                break;
            case ToastPosition.TopLeft:
                Left = workArea.Left + 16;
                Top = workArea.Top + 16;
                break;
            case ToastPosition.BottomRight:
                Left = workArea.Right - ActualWidth - 16;
                Top = workArea.Bottom - ActualHeight - 16;
                break;
            case ToastPosition.BottomLeft:
                Left = workArea.Left + 16;
                Top = workArea.Bottom - ActualHeight - 16;
                break;
            case ToastPosition.TopCenter:
                Left = (workArea.Width - ActualWidth) / 2;
                Top = workArea.Top + 16;
                break;
            case ToastPosition.BottomCenter:
                Left = (workArea.Width - ActualWidth) / 2;
                Top = workArea.Bottom - ActualHeight - 16;
                break;
        }
    }

    /// <inheritdoc/>
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        UpdatePosition();
    }
}
