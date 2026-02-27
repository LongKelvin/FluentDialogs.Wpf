using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;

namespace FluentDialogs.Demo.ViewModels;

/// <summary>
/// ViewModel demonstrating all FluentDialogs.Wpf theming capabilities:
/// custom presets, accent colors, individual token overrides, and live preview.
/// </summary>
public partial class ThemingViewModel : ObservableObject
{
    private readonly IMessageBoxService _messageBox;
    private readonly IFluentDialogThemeService _themeService;
    private readonly IToastService _toast;

    public ThemingViewModel(
        IMessageBoxService messageBox,
        IFluentDialogThemeService themeService,
        IToastService toast)
    {
        _messageBox = messageBox;
        _themeService = themeService;
        _toast = toast;

        // Subscribe to theme changes
        _themeService.PresetChanged += OnPresetChanged;
    }

    // ═══════════════════════════════════════════════════════════════
    // Observable Properties
    // ═══════════════════════════════════════════════════════════════

    [ObservableProperty]
    private string _activePresetName = "Light (Default)";

    [ObservableProperty]
    private string _lastAction = "No theming action yet";

    [ObservableProperty]
    private string _accentColorHex = "#0078D4";

    [ObservableProperty]
    private string _tokenOverrideKey = "FDSemInteractiveDefault";

    [ObservableProperty]
    private string _tokenOverrideValue = "#6B2FA0";

    // ═══════════════════════════════════════════════════════════════
    // Section 1: Built-in Presets (Light / Dark)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Switches to the built-in Light preset.
    /// This is the simplest theming operation — one line of code.
    /// </summary>
    [RelayCommand]
    private void ApplyLightPreset()
    {
        // === HOW TO: Switch to Light theme ===
        _themeService.ApplyPreset(MessageBoxTheme.Light);

        ActivePresetName = "Light (Default)";
        LastAction = "Applied built-in Light preset via ApplyPreset(MessageBoxTheme.Light)";
    }

    /// <summary>
    /// Switches to the built-in Dark preset.
    /// </summary>
    [RelayCommand]
    private void ApplyDarkPreset()
    {
        // === HOW TO: Switch to Dark theme ===
        _themeService.ApplyPreset(MessageBoxTheme.Dark);

        ActivePresetName = "Dark";
        LastAction = "Applied built-in Dark preset via ApplyPreset(MessageBoxTheme.Dark)";
    }

    // ═══════════════════════════════════════════════════════════════
    // Section 2: Custom Presets (XAML ResourceDictionary files)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Applies the Ocean Blue custom preset.
    /// Demonstrates loading a customer-defined XAML preset at runtime.
    /// </summary>
    [RelayCommand]
    private void ApplyOceanBluePreset()
    {
        // === HOW TO: Apply a custom preset from your app's resources ===
        // 1. Create a XAML file with all FDSem* color overrides (see Demo/Themes/OceanBlue.xaml)
        // 2. Call ApplyCustomPreset() with the pack URI
        var presetUri = new Uri(
            "pack://application:,,,/FluentDialogs.Demo;component/Themes/OceanBlue.xaml");

        _themeService.ApplyCustomPreset(presetUri, "Ocean Blue");

        ActivePresetName = "Ocean Blue (Custom)";
        LastAction = "Applied custom preset: OceanBlue.xaml via ApplyCustomPreset(uri, name)";
    }

    /// <summary>
    /// Applies the Sunset Orange custom preset.
    /// </summary>
    [RelayCommand]
    private void ApplySunsetOrangePreset()
    {
        var presetUri = new Uri(
            "pack://application:,,,/FluentDialogs.Demo;component/Themes/SunsetOrange.xaml");

        _themeService.ApplyCustomPreset(presetUri, "Sunset Orange");

        ActivePresetName = "Sunset Orange (Custom)";
        LastAction = "Applied custom preset: SunsetOrange.xaml";
    }

    /// <summary>
    /// Applies the Forest Green custom preset.
    /// </summary>
    [RelayCommand]
    private void ApplyForestGreenPreset()
    {
        var presetUri = new Uri(
            "pack://application:,,,/FluentDialogs.Demo;component/Themes/ForestGreen.xaml");

        _themeService.ApplyCustomPreset(presetUri, "Forest Green");

        ActivePresetName = "Forest Green (Custom)";
        LastAction = "Applied custom preset: ForestGreen.xaml";
    }

    /// <summary>
    /// Applies the Royal Purple custom preset.
    /// </summary>
    [RelayCommand]
    private void ApplyRoyalPurplePreset()
    {
        var presetUri = new Uri(
            "pack://application:,,,/FluentDialogs.Demo;component/Themes/RoyalPurple.xaml");

        _themeService.ApplyCustomPreset(presetUri, "Royal Purple");

        ActivePresetName = "Royal Purple (Custom)";
        LastAction = "Applied custom preset: RoyalPurple.xaml";
    }

    /// <summary>
    /// Applies the Midnight Teal custom dark preset.
    /// </summary>
    [RelayCommand]
    private void ApplyMidnightTealPreset()
    {
        var presetUri = new Uri(
            "pack://application:,,,/FluentDialogs.Demo;component/Themes/MidnightTeal.xaml");

        _themeService.ApplyCustomPreset(presetUri, "Midnight Teal");

        ActivePresetName = "Midnight Teal (Custom Dark)";
        LastAction = "Applied custom dark preset: MidnightTeal.xaml";
    }

    // ═══════════════════════════════════════════════════════════════
    // Section 3: Accent Color — Brand Color Override
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Sets the accent color from the hex input.
    /// Accent color automatically derives hover and pressed states.
    /// </summary>
    [RelayCommand]
    private void ApplyAccentColor()
    {
        try
        {
            // === HOW TO: Set brand/accent color ===
            // This is the easiest way to brand dialogs — just provide your primary color.
            // The service automatically derives hover (-15%) and pressed (-30%) states.
            var color = (Color)ColorConverter.ConvertFromString(AccentColorHex);
            _themeService.SetAccentColor(color);

            LastAction = $"Set accent color to {AccentColorHex} — hover/pressed states auto-derived";
            _toast.ShowSuccess($"Accent color set to {AccentColorHex}");
        }
        catch (FormatException)
        {
            _toast.ShowError($"Invalid color: {AccentColorHex}");
        }
    }

    /// <summary>Quick accent: Microsoft Blue</summary>
    [RelayCommand]
    private void SetAccentBlue()
    {
        AccentColorHex = "#0078D4";
        var color = Color.FromRgb(0x00, 0x78, 0xD4);
        _themeService.SetAccentColor(color);
        LastAction = "Accent → Microsoft Blue #0078D4";
    }

    /// <summary>Quick accent: Vibrant Orange</summary>
    [RelayCommand]
    private void SetAccentOrange()
    {
        AccentColorHex = "#D83B01";
        var color = Color.FromRgb(0xD8, 0x3B, 0x01);
        _themeService.SetAccentColor(color);
        LastAction = "Accent → Vibrant Orange #D83B01";
    }

    /// <summary>Quick accent: Teal</summary>
    [RelayCommand]
    private void SetAccentTeal()
    {
        AccentColorHex = "#008272";
        var color = Color.FromRgb(0x00, 0x82, 0x72);
        _themeService.SetAccentColor(color);
        LastAction = "Accent → Teal #008272";
    }

    /// <summary>Quick accent: Rose</summary>
    [RelayCommand]
    private void SetAccentRose()
    {
        AccentColorHex = "#E3008C";
        var color = Color.FromRgb(0xE3, 0x00, 0x8C);
        _themeService.SetAccentColor(color);
        LastAction = "Accent → Rose #E3008C";
    }

    // ═══════════════════════════════════════════════════════════════
    // Section 4: Individual Token Overrides
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Overrides a single semantic token at runtime.
    /// </summary>
    [RelayCommand]
    private void ApplyTokenOverride()
    {
        try
        {
            // === HOW TO: Override a single semantic token ===
            // Use ThemeTokenKeys constants to avoid magic strings.
            // Overrides persist across preset switches until ClearOverrides().
            var color = (Color)ColorConverter.ConvertFromString(TokenOverrideValue);
            _themeService.SetToken(TokenOverrideKey, color);

            LastAction = $"Token override: {TokenOverrideKey} = {TokenOverrideValue}";
            _toast.ShowSuccess($"Token {TokenOverrideKey} overridden");
        }
        catch (FormatException)
        {
            _toast.ShowError($"Invalid color: {TokenOverrideValue}");
        }
    }

    /// <summary>
    /// Clears all runtime overrides, reverting to the active preset's values.
    /// </summary>
    [RelayCommand]
    private void ClearAllOverrides()
    {
        // === HOW TO: Clear all runtime overrides ===
        _themeService.ClearOverrides();

        LastAction = "Cleared all runtime overrides — reverted to active preset values";
        _toast.ShowInfo("All overrides cleared");
    }

    /// <summary>
    /// Resets everything back to the built-in Light preset.
    /// </summary>
    [RelayCommand]
    private void ResetToDefault()
    {
        _themeService.ClearOverrides();
        _themeService.ApplyPreset(MessageBoxTheme.Light);

        ActivePresetName = "Light (Default)";
        AccentColorHex = "#0078D4";
        LastAction = "Reset to default Light preset with all overrides cleared";
        _toast.ShowInfo("Reset to default");
    }

    // ═══════════════════════════════════════════════════════════════
    // Section 5: Preview — Show Themed Dialogs
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Shows an info dialog to preview the current theme.
    /// </summary>
    [RelayCommand]
    private async Task PreviewInfoDialogAsync()
    {
        await _messageBox.InfoAsync(
            "This informational dialog reflects the current theme settings. " +
            "Notice the surface color, text color, and button styling.",
            "Themed Info Dialog");
    }

    /// <summary>
    /// Shows a confirmation dialog to preview the current theme.
    /// </summary>
    [RelayCommand]
    private async Task PreviewConfirmDialogAsync()
    {
        await _messageBox.ConfirmAsync(
            "This confirmation dialog uses the active preset's accent color on the primary button. " +
            "The secondary button uses the neutral token.",
            "Themed Confirmation");
    }

    /// <summary>
    /// Shows an error dialog to preview status colors.
    /// </summary>
    [RelayCommand]
    private async Task PreviewErrorDialogAsync()
    {
        await _messageBox.ErrorAsync(
            "Error dialogs use FDSemStatusError tokens for the icon and danger button. " +
            "These stay red-ish even in custom presets for safety.");
    }

    /// <summary>
    /// Shows a warning dialog to preview status colors.
    /// </summary>
    [RelayCommand]
    private async Task PreviewWarningDialogAsync()
    {
        await _messageBox.WarningAsync(
            "Warning dialogs use FDSemStatusWarning for the icon color.",
            "Themed Warning");
    }

    /// <summary>
    /// Shows all icon types in sequence to preview the theme.
    /// </summary>
    [RelayCommand]
    private async Task PreviewAllIconsAsync()
    {
        var options = new MessageBoxOptions
        {
            Title = "All Icons Preview",
            Message = "This dialog uses the Info icon. After closing, you'll see Warning, Error, Success, and Question icons.",
            Icon = MessageBoxIcon.Info,
            Buttons = MessageBoxButtons.OK
        };
        await _messageBox.ShowAsync(options);

        options = new MessageBoxOptions
        {
            Title = "Warning Icon",
            Message = "Warning icon with the current theme tokens.",
            Icon = MessageBoxIcon.Warning,
            Buttons = MessageBoxButtons.OK
        };
        await _messageBox.ShowAsync(options);

        options = new MessageBoxOptions
        {
            Title = "Error Icon",
            Message = "Error icon — status colors are preserved for safety.",
            Icon = MessageBoxIcon.Error,
            Buttons = MessageBoxButtons.OK
        };
        await _messageBox.ShowAsync(options);

        options = new MessageBoxOptions
        {
            Title = "Success Icon",
            Message = "Success icon with the current theme tokens.",
            Icon = MessageBoxIcon.Success,
            Buttons = MessageBoxButtons.OK
        };
        await _messageBox.ShowAsync(options);

        options = new MessageBoxOptions
        {
            Title = "Question Icon",
            Message = "Question icon — uses the Interactive accent color.",
            Icon = MessageBoxIcon.Question,
            Buttons = MessageBoxButtons.YesNo
        };
        await _messageBox.ShowAsync(options);
    }

    /// <summary>
    /// Shows a dialog with custom buttons to preview button styles.
    /// </summary>
    [RelayCommand]
    private async Task PreviewCustomButtonsAsync()
    {
        var buttons = new List<MessageBoxButtonDefinition>
        {
            new()
            {
                Text = "Primary Action",
                Result = MessageBoxResult.Yes,
                Style = ButtonStyle.Primary,
                IsDefault = true
            },
            new()
            {
                Text = "Danger Action",
                Result = MessageBoxResult.No,
                Style = ButtonStyle.Danger
            },
            new()
            {
                Text = "Cancel",
                Result = MessageBoxResult.Cancel,
                Style = ButtonStyle.Default,
                IsCancel = true
            }
        };

        var options = new MessageBoxOptions
        {
            Title = "Button Styles Preview",
            Message = "This dialog shows Primary, Danger, and Default button styles with the current theme. " +
                      "Primary uses the Interactive accent, Danger uses StatusError, Default uses Neutral tokens.",
            Icon = MessageBoxIcon.Info,
            CustomButtons = buttons
        };

        await _messageBox.ShowAsync(options);
    }

    /// <summary>
    /// Shows a builder dialog with the current theme.
    /// </summary>
    [RelayCommand]
    private async Task PreviewBuilderDialogAsync()
    {
        await _messageBox.Confirm("This confirmation dialog was built with the Fluent Builder API. " +
                                   "It inherits all theming from the active preset.", "Builder Preview")
            .OnYes(() => _toast.ShowSuccess("You clicked Yes!"))
            .OnNo(() => _toast.ShowInfo("You clicked No"))
            .ShowAsync();
    }

    /// <summary>
    /// Shows a themed input dialog.
    /// </summary>
    [RelayCommand]
    private async Task PreviewInputDialogAsync()
    {
        var result = await _messageBox.InputAsync(
            "Input fields also use the theme tokens for borders, backgrounds, and focus states.",
            "Enter something...",
            title: "Themed Input");

        if (result.Result == MessageBoxResult.OK)
        {
            _toast.ShowSuccess($"You entered: \"{result.InputText}\"");
        }
    }

    /// <summary>
    /// Shows a dropdown dialog with the current theme.
    /// </summary>
    [RelayCommand]
    private async Task PreviewDropdownDialogAsync()
    {
        var items = new[] { "Option A", "Option B", "Option C", "Option D" };

        var result = await _messageBox.DropdownAsync(
            "The ComboBox dropdown also inherits the active theme tokens.",
            items,
            defaultIndex: 0,
            title: "Themed Dropdown");

        if (result.Result == MessageBoxResult.OK)
        {
            _toast.ShowSuccess($"Selected: {result.DropdownSelectedItem}");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // Event Handlers
    // ═══════════════════════════════════════════════════════════════

    private void OnPresetChanged(object? sender, ThemePresetChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.CustomPresetName))
        {
            ActivePresetName = $"{e.CustomPresetName} (Custom)";
        }
    }
}
