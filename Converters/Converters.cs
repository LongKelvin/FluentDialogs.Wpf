using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FluentDialogs.Converters;

/// <summary>
/// Converts an empty or null string to Visibility.Visible, otherwise Collapsed.
/// Used to show placeholder text when input is empty.
/// </summary>
public sealed class EmptyStringToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a string value to Visibility.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var str = value as string;
        return string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean to Visibility with optional inversion.
/// Pass "Inverse" as ConverterParameter to invert the result.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to Visibility.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var boolValue = value is bool b && b;
        var inverse = parameter is string s && s.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
        
        if (inverse)
        {
            boolValue = !boolValue;
        }

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverts a boolean value.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{
    /// <summary>
    /// Inverts the boolean value.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    /// <summary>
    /// Inverts the boolean value back.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}

/// <summary>
/// Converts a size value to SizeToContent enum.
/// Returns Manual if a size is specified, otherwise Height/Width based on parameter.
/// </summary>
public sealed class SizeToContentConverter : IValueConverter
{
    /// <summary>
    /// Converts size value to SizeToContent.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // If a size is specified, use Manual sizing
        if (value is double d && !double.IsNaN(d))
        {
            return SizeToContent.Manual;
        }

        // If no size specified, auto-size to content
        var param = parameter as string;
        return param switch
        {
            "Width" => SizeToContent.Width,
            "Height" => SizeToContent.Height,
            "WidthAndHeight" => SizeToContent.WidthAndHeight,
            _ => SizeToContent.Height
        };
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a color to its contrasting color (white or black) for text readability.
/// </summary>
public sealed class ColorContrastConverter : IValueConverter
{
    /// <summary>
    /// Converts a color to its contrasting color.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color)
        {
            return Colors.Black; // Default to black if no color provided
        }

        // Calculate relative luminance
        var luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

        // Return white for dark colors, black for light colors
        return new SolidColorBrush(luminance > 0.5 ? Colors.Black : Colors.White);
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
