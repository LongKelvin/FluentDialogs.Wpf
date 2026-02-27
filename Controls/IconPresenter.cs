using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentDialogs.Enums;

namespace FluentDialogs.Controls;

/// <summary>
/// A control that displays modern Fluent Design icons for message box dialogs.
/// Icons use a filled circular background with white inner symbols,
/// matching Windows 11 Fluent Design guidelines.
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

    /// <summary>
    /// Identifies the <see cref="IconDrawing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconDrawingProperty =
        DependencyProperty.Register(
            nameof(IconDrawing),
            typeof(Drawing),
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
    /// When set, this overrides the built-in icon drawing.
    /// </summary>
    public Geometry? IconContent
    {
        get => (Geometry?)GetValue(IconContentProperty);
        set => SetValue(IconContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the composite drawing for the icon.
    /// This contains the filled circle + white symbol overlay.
    /// </summary>
    public Drawing? IconDrawing
    {
        get => (Drawing?)GetValue(IconDrawingProperty);
        set => SetValue(IconDrawingProperty, value);
    }

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IconPresenter presenter && presenter.IconContent == null)
        {
            presenter.UpdateIconDrawing();
        }
    }

    private void UpdateIconDrawing()
    {
        IconDrawing = Icon switch
        {
            MessageBoxIcon.Info => CreateInfoDrawing(),
            MessageBoxIcon.Warning => CreateWarningDrawing(),
            MessageBoxIcon.Error => CreateErrorDrawing(),
            MessageBoxIcon.Success => CreateSuccessDrawing(),
            MessageBoxIcon.Question => CreateQuestionDrawing(),
            MessageBoxIcon.None => CreateInfoDrawing(),
            _ => CreateInfoDrawing()
        };
    }

    // ─── Shared helpers ───────────────────────────────────────────

    private static Pen WhitePen(double thickness = 2.0) =>
        new(Brushes.White, thickness) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round };

    private static GeometryDrawing FilledCircle(Brush fill, double cx, double cy, double r) =>
        new(fill, null, new EllipseGeometry(new Point(cx, cy), r, r));

    // ─── Info Icon: blue circle, white "i" ────────────────────────

    private static Drawing CreateInfoDrawing()
    {
        var group = new DrawingGroup();

        // Blue circle
        group.Children.Add(FilledCircle(new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4)), 16, 16, 15));

        // "i" dot
        group.Children.Add(new GeometryDrawing(Brushes.White, null,
            new EllipseGeometry(new Point(16, 9.5), 1.8, 1.8)));

        // "i" stem
        var stemFigure = new PathFigure { StartPoint = new Point(16, 14), IsClosed = false, IsFilled = false };
        stemFigure.Segments.Add(new LineSegment(new Point(16, 23), true));
        var stemGeom = new PathGeometry([stemFigure]);
        group.Children.Add(new GeometryDrawing(null, WhitePen(2.4), stemGeom));

        group.Freeze();
        return group;
    }

    // ─── Warning Icon: amber/yellow filled rounded triangle, white "!" ─

    private static Drawing CreateWarningDrawing()
    {
        var group = new DrawingGroup();

        // Rounded triangle background
        var triFigure = new PathFigure { StartPoint = new Point(16, 1.5), IsClosed = true, IsFilled = true };
        triFigure.Segments.Add(new BezierSegment(new Point(17.2, 1.5), new Point(18.3, 2.2), new Point(18.9, 3.3), true));
        triFigure.Segments.Add(new LineSegment(new Point(30, 25.5), true));
        triFigure.Segments.Add(new BezierSegment(new Point(31.1, 27.5), new Point(29.7, 30), new Point(27.5, 30), true));
        triFigure.Segments.Add(new LineSegment(new Point(4.5, 30), true));
        triFigure.Segments.Add(new BezierSegment(new Point(2.3, 30), new Point(0.9, 27.5), new Point(2, 25.5), true));
        triFigure.Segments.Add(new LineSegment(new Point(13.1, 3.3), true));
        triFigure.Segments.Add(new BezierSegment(new Point(13.7, 2.2), new Point(14.8, 1.5), new Point(16, 1.5), true));
        var triGeom = new PathGeometry([triFigure]);
        group.Children.Add(new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0xF7, 0x96, 0x0A)), null, triGeom));

        // Exclamation stem
        var stemFigure = new PathFigure { StartPoint = new Point(16, 11), IsClosed = false, IsFilled = false };
        stemFigure.Segments.Add(new LineSegment(new Point(16, 21), true));
        group.Children.Add(new GeometryDrawing(null, WhitePen(2.4), new PathGeometry([stemFigure])));

        // Exclamation dot
        group.Children.Add(new GeometryDrawing(Brushes.White, null,
            new EllipseGeometry(new Point(16, 25), 1.8, 1.8)));

        group.Freeze();
        return group;
    }

    // ─── Error Icon: red circle, white "×" ────────────────────────

    private static Drawing CreateErrorDrawing()
    {
        var group = new DrawingGroup();

        // Red circle — Fluent #D13438
        group.Children.Add(FilledCircle(new SolidColorBrush(Color.FromRgb(0xD1, 0x34, 0x38)), 16, 16, 15));

        // White "×" — two diagonal lines with rounded caps
        var pen = WhitePen(2.4);

        var line1 = new PathFigure { StartPoint = new Point(10.5, 10.5), IsClosed = false, IsFilled = false };
        line1.Segments.Add(new LineSegment(new Point(21.5, 21.5), true));

        var line2 = new PathFigure { StartPoint = new Point(21.5, 10.5), IsClosed = false, IsFilled = false };
        line2.Segments.Add(new LineSegment(new Point(10.5, 21.5), true));

        var crossGeom = new PathGeometry([line1, line2]);
        group.Children.Add(new GeometryDrawing(null, pen, crossGeom));

        group.Freeze();
        return group;
    }

    // ─── Success Icon: green circle, white checkmark ──────────────

    private static Drawing CreateSuccessDrawing()
    {
        var group = new DrawingGroup();

        // Green circle — Fluent #107C10
        group.Children.Add(FilledCircle(new SolidColorBrush(Color.FromRgb(0x10, 0x7C, 0x10)), 16, 16, 15));

        // White checkmark with rounded stroke
        var checkFigure = new PathFigure { StartPoint = new Point(9, 16.5), IsClosed = false, IsFilled = false };
        checkFigure.Segments.Add(new LineSegment(new Point(13.5, 21.5), true));
        checkFigure.Segments.Add(new LineSegment(new Point(23, 10.5), true));
        var checkGeom = new PathGeometry([checkFigure]);
        group.Children.Add(new GeometryDrawing(null, WhitePen(2.4), checkGeom));

        group.Freeze();
        return group;
    }

    // ─── Question Icon: blue circle, white "?" ────────────────────

    private static Drawing CreateQuestionDrawing()
    {
        var group = new DrawingGroup();

        // Blue circle — Fluent #0078D4
        group.Children.Add(FilledCircle(new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4)), 16, 16, 15));

        // "?" curve
        var qFigure = new PathFigure { StartPoint = new Point(12, 12), IsClosed = false, IsFilled = false };
        qFigure.Segments.Add(new BezierSegment(
            new Point(12, 8.5), new Point(14, 7), new Point(16, 7), true));
        qFigure.Segments.Add(new BezierSegment(
            new Point(18.5, 7), new Point(20.5, 8.5), new Point(20.5, 11), true));
        qFigure.Segments.Add(new BezierSegment(
            new Point(20.5, 13), new Point(18.5, 14.5), new Point(16, 17), true));

        var qGeom = new PathGeometry([qFigure]);
        group.Children.Add(new GeometryDrawing(null, WhitePen(2.2), qGeom));

        // "?" dot
        group.Children.Add(new GeometryDrawing(Brushes.White, null,
            new EllipseGeometry(new Point(16, 22), 1.8, 1.8)));

        group.Freeze();
        return group;
    }
}
