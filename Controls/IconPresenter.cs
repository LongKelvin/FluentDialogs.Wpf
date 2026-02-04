using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentDialogs.Enums;

namespace FluentDialogs.Controls;

/// <summary>
/// A control that displays icons for message box dialogs using vector geometry.
/// </summary>
public class IconPresenter : Control
{
    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(MessageBoxIcon),
            typeof(IconPresenter),
            new PropertyMetadata(MessageBoxIcon.None, OnIconChanged));

    /// <summary>
    /// Identifies the <see cref="IconContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconContentProperty =
        DependencyProperty.Register(
            nameof(IconContent),
            typeof(Geometry),
            typeof(IconPresenter),
            new PropertyMetadata(null));

    static IconPresenter()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IconPresenter),
            new FrameworkPropertyMetadata(typeof(IconPresenter)));
    }

    /// <summary>
    /// Gets or sets the icon to display.
    /// </summary>
    public MessageBoxIcon Icon
    {
        get => (MessageBoxIcon)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets custom icon geometry to display.
    /// When set, this overrides the <see cref="Icon"/> property.
    /// </summary>
    public Geometry? IconContent
    {
        get => (Geometry?)GetValue(IconContentProperty);
        set => SetValue(IconContentProperty, value);
    }

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IconPresenter presenter && presenter.IconContent == null)
        {
            presenter.UpdateIconGeometry();
        }
    }

    private void UpdateIconGeometry()
    {
        IconContent = Icon switch
        {
            MessageBoxIcon.Info => CreateInfoGeometry(),
            MessageBoxIcon.Warning => CreateWarningGeometry(),
            MessageBoxIcon.Error => CreateErrorGeometry(),
            MessageBoxIcon.Success => CreateSuccessGeometry(),
            MessageBoxIcon.Question => CreateQuestionGeometry(),
            MessageBoxIcon.None => CreateInfoGeometry(), // Default to Info icon
            _ => CreateInfoGeometry()
        };
    }

    private static Geometry CreateInfoGeometry()
    {
        var group = new GeometryGroup();
        
        var circle = new EllipseGeometry(new Point(16, 16), 14, 14);
        group.Children.Add(circle);
        
        var iDot = new EllipseGeometry(new Point(16, 10), 2, 2);
        group.Children.Add(iDot);
        
        var iLine = new RectangleGeometry(new Rect(14, 14, 4, 10));
        group.Children.Add(iLine);
        
        return group;
    }

    private static Geometry CreateWarningGeometry()
    {
        var pathGeometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(16, 2) };
        
        figure.Segments.Add(new LineSegment(new Point(30, 28), true));
        figure.Segments.Add(new LineSegment(new Point(2, 28), true));
        figure.Segments.Add(new LineSegment(new Point(16, 2), true));
        
        pathGeometry.Figures.Add(figure);
        
        var group = new GeometryGroup();
        group.Children.Add(pathGeometry);
        
        var exclamationLine = new RectangleGeometry(new Rect(14, 10, 4, 10));
        group.Children.Add(exclamationLine);
        
        var exclamationDot = new EllipseGeometry(new Point(16, 24), 2, 2);
        group.Children.Add(exclamationDot);
        
        return group;
    }

    private static Geometry CreateErrorGeometry()
    {
        var group = new GeometryGroup();
        
        var circle = new EllipseGeometry(new Point(16, 16), 14, 14);
        group.Children.Add(circle);
        
        var transform = new RotateTransform(45, 16, 16);
        var cross1 = new RectangleGeometry(new Rect(8, 14, 16, 4), 0, 0, transform);
        group.Children.Add(cross1);
        
        var transform2 = new RotateTransform(-45, 16, 16);
        var cross2 = new RectangleGeometry(new Rect(8, 14, 16, 4), 0, 0, transform2);
        group.Children.Add(cross2);
        
        return group;
    }

    private static Geometry CreateSuccessGeometry()
    {
        var group = new GeometryGroup();
        
        var circle = new EllipseGeometry(new Point(16, 16), 14, 14);
        group.Children.Add(circle);
        
        var pathGeometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(8, 16) };
        
        figure.Segments.Add(new LineSegment(new Point(13, 21), true));
        figure.Segments.Add(new LineSegment(new Point(24, 10), true));
        
        pathGeometry.Figures.Add(figure);
        group.Children.Add(pathGeometry);
        
        return group;
    }

    private static Geometry CreateQuestionGeometry()
    {
        var group = new GeometryGroup();
        
        var circle = new EllipseGeometry(new Point(16, 16), 14, 14);
        group.Children.Add(circle);
        
        var pathGeometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(12, 12) };
        
        figure.Segments.Add(new BezierSegment(
            new Point(12, 10),
            new Point(14, 8),
            new Point(16, 8),
            true));
        figure.Segments.Add(new BezierSegment(
            new Point(18, 8),
            new Point(20, 10),
            new Point(20, 12),
            true));
        figure.Segments.Add(new LineSegment(new Point(20, 14), true));
        figure.Segments.Add(new LineSegment(new Point(16, 18), true));
        
        pathGeometry.Figures.Add(figure);
        group.Children.Add(pathGeometry);
        
        var questionDot = new EllipseGeometry(new Point(16, 22), 2, 2);
        group.Children.Add(questionDot);
        
        return group;
    }
}
