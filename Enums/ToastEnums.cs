namespace FluentDialogs.Enums;

/// <summary>
/// Specifies the visual style and icon for a toast notification.
/// </summary>
public enum ToastType
{
    /// <summary>
    /// Informational toast with blue accent.
    /// </summary>
    Info,

    /// <summary>
    /// Success toast with green accent.
    /// </summary>
    Success,

    /// <summary>
    /// Warning toast with yellow/orange accent.
    /// </summary>
    Warning,

    /// <summary>
    /// Error toast with red accent.
    /// </summary>
    Error
}

/// <summary>
/// Specifies the position where toast notifications appear on screen.
/// </summary>
public enum ToastPosition
{
    /// <summary>
    /// Toast appears at the top-right corner.
    /// </summary>
    TopRight,

    /// <summary>
    /// Toast appears at the top-left corner.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Toast appears at the bottom-right corner.
    /// </summary>
    BottomRight,

    /// <summary>
    /// Toast appears at the bottom-left corner.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Toast appears at the top-center.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Toast appears at the bottom-center.
    /// </summary>
    BottomCenter
}
