using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FluentDialogs.Enums;
using FluentDialogs.Models;

namespace FluentDialogs.ViewModels;

/// <summary>
/// ViewModel for a message box button.
/// </summary>
public sealed class MessageBoxButtonViewModel : INotifyPropertyChanged
{
    private readonly MessageBoxButtonDefinition _definition;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBoxButtonViewModel"/> class.
    /// </summary>
    /// <param name="definition">The button definition.</param>
    /// <param name="clickCommand">The command to execute when the button is clicked.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> or <paramref name="clickCommand"/> is null.</exception>
    public MessageBoxButtonViewModel(MessageBoxButtonDefinition definition, ICommand clickCommand)
    {
        _definition = definition ?? throw new ArgumentNullException(nameof(definition));
        ClickCommand = clickCommand ?? throw new ArgumentNullException(nameof(clickCommand));
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the text displayed on the button.
    /// </summary>
    public string Text => _definition.Text;

    /// <summary>
    /// Gets the visual style of the button.
    /// </summary>
    public ButtonStyle Style => _definition.Style;

    /// <summary>
    /// Gets a value indicating whether this button is the default button.
    /// </summary>
    public bool IsDefault => _definition.IsDefault;

    /// <summary>
    /// Gets a value indicating whether this button is the cancel button.
    /// </summary>
    public bool IsCancel => _definition.IsCancel;

    /// <summary>
    /// Gets the result value returned when this button is clicked.
    /// </summary>
    public MessageBoxResult Result => _definition.Result;

    /// <summary>
    /// Gets the command to execute when the button is clicked.
    /// </summary>
    public ICommand ClickCommand { get; }

    /// <summary>
    /// Gets the custom command defined in the button definition.
    /// </summary>
    internal ICommand? CustomCommand => _definition.Command;

    /// <summary>
    /// Gets the parameter to pass to the custom command.
    /// </summary>
    internal object? CustomCommandParameter => _definition.CommandParameter;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
