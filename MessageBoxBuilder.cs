using System.Windows;
using System.Windows.Media;
using FluentDialogs.Abstractions;
using FluentDialogs.Enums;
using FluentDialogs.Models;
using MessageBoxOptions = FluentDialogs.Models.MessageBoxOptions;

namespace FluentDialogs;

/// <summary>
/// Fluent builder for creating and configuring message box dialogs.
/// </summary>
/// <remarks>
/// Provides a chainable API for configuring dialogs with callbacks for button actions.
/// </remarks>
/// <example>
/// <code>
/// await MessageBoxBuilder.Create(_messageBoxService)
///     .WithTitle("Confirm Delete")
///     .WithMessage("Are you sure you want to delete this item?")
///     .WithIcon(MessageBoxIcon.Warning)
///     .WithButtons(MessageBoxButtons.YesNo)
///     .OnYes(() => DeleteItem())
///     .OnNo(() => Console.WriteLine("Cancelled"))
///     .ShowAsync();
/// </code>
/// </example>
public sealed class MessageBoxBuilder
{
    private readonly IMessageBoxService _service;
    private string _title = string.Empty;
    private string _message = string.Empty;
    private MessageBoxIcon _icon = MessageBoxIcon.None;
    private MessageBoxButtons? _buttons;
    private List<MessageBoxButtonDefinition>? _customButtons;
    private Window? _owner;
    private object? _content;
    private string? _checkboxText;
    private bool _checkboxChecked;
    private string? _inputPlaceholder;
    private string? _inputDefaultValue;
    private bool _inputIsPassword;
    private Color? _titleBarColor;
    private double? _width;
    private double? _height;

    private Action? _onYes;
    private Action? _onNo;
    private Action? _onOk;
    private Action? _onCancel;
    private Action<DialogResult>? _onResult;

    private MessageBoxBuilder(IMessageBoxService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Creates a new message box builder instance.
    /// </summary>
    /// <param name="service">The message box service to use for displaying the dialog.</param>
    /// <returns>A new builder instance.</returns>
    public static MessageBoxBuilder Create(IMessageBoxService service) => new(service);

    /// <summary>
    /// Sets the dialog title.
    /// </summary>
    /// <param name="title">The title text.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>
    /// Sets the dialog message.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    /// <summary>
    /// Sets the dialog icon.
    /// </summary>
    /// <param name="icon">The icon to display.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithIcon(MessageBoxIcon icon)
    {
        _icon = icon;
        return this;
    }

    /// <summary>
    /// Sets the standard buttons to display.
    /// </summary>
    /// <param name="buttons">The button configuration.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithButtons(MessageBoxButtons buttons)
    {
        _buttons = buttons;
        _customButtons = null;
        return this;
    }

    /// <summary>
    /// Sets custom button definitions.
    /// </summary>
    /// <param name="buttons">The custom button definitions.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithCustomButtons(params MessageBoxButtonDefinition[] buttons)
    {
        _customButtons = [.. buttons];
        _buttons = null;
        return this;
    }

    /// <summary>
    /// Sets the owner window for modal behavior.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithOwner(Window owner)
    {
        _owner = owner;
        return this;
    }

    /// <summary>
    /// Sets custom content to display in the dialog.
    /// </summary>
    /// <param name="content">The content to display.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithContent(object content)
    {
        _content = content;
        return this;
    }

    /// <summary>
    /// Adds a checkbox to the dialog.
    /// </summary>
    /// <param name="text">The checkbox label text.</param>
    /// <param name="isChecked">The initial checked state.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithCheckbox(string text, bool isChecked = false)
    {
        _checkboxText = text;
        _checkboxChecked = isChecked;
        return this;
    }

    /// <summary>
    /// Adds an input field to the dialog.
    /// </summary>
    /// <param name="placeholder">The placeholder text.</param>
    /// <param name="defaultValue">The initial value.</param>
    /// <param name="isPassword">Whether to mask input as a password.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithInput(string placeholder, string? defaultValue = null, bool isPassword = false)
    {
        _inputPlaceholder = placeholder;
        _inputDefaultValue = defaultValue;
        _inputIsPassword = isPassword;
        return this;
    }

    /// <summary>
    /// Sets a custom title bar color.
    /// </summary>
    /// <param name="color">The title bar color.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithTitleBarColor(Color color)
    {
        _titleBarColor = color;
        return this;
    }

    /// <summary>
    /// Sets the dialog dimensions.
    /// </summary>
    /// <param name="width">The dialog width.</param>
    /// <param name="height">The dialog height.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder WithSize(double? width = null, double? height = null)
    {
        _width = width;
        _height = height;
        return this;
    }

    /// <summary>
    /// Registers a callback for when the user clicks Yes.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder OnYes(Action action)
    {
        _onYes = action;
        return this;
    }

    /// <summary>
    /// Registers a callback for when the user clicks No.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder OnNo(Action action)
    {
        _onNo = action;
        return this;
    }

    /// <summary>
    /// Registers a callback for when the user clicks OK.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder OnOk(Action action)
    {
        _onOk = action;
        return this;
    }

    /// <summary>
    /// Registers a callback for when the user clicks Cancel or closes the dialog.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder OnCancel(Action action)
    {
        _onCancel = action;
        return this;
    }

    /// <summary>
    /// Registers a callback that receives the full dialog result.
    /// </summary>
    /// <param name="action">The action to execute with the result.</param>
    /// <returns>The builder for chaining.</returns>
    public MessageBoxBuilder OnResult(Action<DialogResult> action)
    {
        _onResult = action;
        return this;
    }

    /// <summary>
    /// Displays the dialog and executes the appropriate callback based on the result.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains the dialog result.</returns>
    public async Task<DialogResult> ShowAsync()
    {
        var options = BuildOptions();
        var result = await _service.ShowExtendedAsync(options);

        // Execute callbacks based on result
        _onResult?.Invoke(result);

        switch (result.Result)
        {
            case MessageBoxResult.Yes:
                _onYes?.Invoke();
                break;
            case MessageBoxResult.No:
                _onNo?.Invoke();
                break;
            case MessageBoxResult.OK:
                _onOk?.Invoke();
                break;
            case MessageBoxResult.Cancel:
            case MessageBoxResult.None:
                _onCancel?.Invoke();
                break;
        }

        return result;
    }

    /// <summary>
    /// Builds the MessageBoxOptions from the current builder configuration.
    /// </summary>
    /// <returns>The configured options.</returns>
    public MessageBoxOptions BuildOptions()
    {
        // Default to OK button if none specified
        if (!_buttons.HasValue && (_customButtons == null || _customButtons.Count == 0))
        {
            _buttons = MessageBoxButtons.OK;
        }

        return new MessageBoxOptions
        {
            Title = _title,
            Message = _message,
            Icon = _icon,
            Buttons = _buttons,
            CustomButtons = _customButtons?.AsReadOnly(),
            Owner = _owner,
            Content = _content,
            CheckboxText = _checkboxText,
            CheckboxChecked = _checkboxChecked,
            InputPlaceholder = _inputPlaceholder,
            InputDefaultValue = _inputDefaultValue,
            InputIsPassword = _inputIsPassword,
            TitleBarColor = _titleBarColor,
            Width = _width,
            Height = _height
        };
    }
}

/// <summary>
/// Extension methods for creating fluent message box builders.
/// </summary>
public static class MessageBoxBuilderExtensions
{
    /// <summary>
    /// Creates a confirmation dialog builder.
    /// </summary>
    /// <param name="service">The message box service.</param>
    /// <param name="message">The confirmation message.</param>
    /// <param name="title">The optional title.</param>
    /// <returns>A builder configured for confirmation.</returns>
    /// <example>
    /// <code>
    /// await _messageBoxService.Confirm("Delete this item?")
    ///     .OnYes(() => DeleteItem())
    ///     .ShowAsync();
    /// </code>
    /// </example>
    public static MessageBoxBuilder Confirm(this IMessageBoxService service, string message, string? title = null)
    {
        return MessageBoxBuilder.Create(service)
            .WithTitle(title ?? "Confirm")
            .WithMessage(message)
            .WithIcon(MessageBoxIcon.Question)
            .WithButtons(MessageBoxButtons.YesNo);
    }

    /// <summary>
    /// Creates an information dialog builder.
    /// </summary>
    /// <param name="service">The message box service.</param>
    /// <param name="message">The information message.</param>
    /// <param name="title">The optional title.</param>
    /// <returns>A builder configured for information.</returns>
    public static MessageBoxBuilder Info(this IMessageBoxService service, string message, string? title = null)
    {
        return MessageBoxBuilder.Create(service)
            .WithTitle(title ?? "Information")
            .WithMessage(message)
            .WithIcon(MessageBoxIcon.Info)
            .WithButtons(MessageBoxButtons.OK);
    }

    /// <summary>
    /// Creates a warning dialog builder.
    /// </summary>
    /// <param name="service">The message box service.</param>
    /// <param name="message">The warning message.</param>
    /// <param name="title">The optional title.</param>
    /// <returns>A builder configured for warning.</returns>
    public static MessageBoxBuilder Warning(this IMessageBoxService service, string message, string? title = null)
    {
        return MessageBoxBuilder.Create(service)
            .WithTitle(title ?? "Warning")
            .WithMessage(message)
            .WithIcon(MessageBoxIcon.Warning)
            .WithButtons(MessageBoxButtons.OK);
    }

    /// <summary>
    /// Creates an error dialog builder.
    /// </summary>
    /// <param name="service">The message box service.</param>
    /// <param name="message">The error message.</param>
    /// <param name="title">The optional title.</param>
    /// <returns>A builder configured for error.</returns>
    public static MessageBoxBuilder Error(this IMessageBoxService service, string message, string? title = null)
    {
        return MessageBoxBuilder.Create(service)
            .WithTitle(title ?? "Error")
            .WithMessage(message)
            .WithIcon(MessageBoxIcon.Error)
            .WithButtons(MessageBoxButtons.OK);
    }

    /// <summary>
    /// Creates an input dialog builder.
    /// </summary>
    /// <param name="service">The message box service.</param>
    /// <param name="message">The prompt message.</param>
    /// <param name="placeholder">The input placeholder text.</param>
    /// <param name="title">The optional title.</param>
    /// <returns>A builder configured for input.</returns>
    /// <example>
    /// <code>
    /// var result = await _messageBoxService.Input("Enter your name:", "Name")
    ///     .OnOk(() => Console.WriteLine("Submitted"))
    ///     .ShowAsync();
    /// string name = result.InputText;
    /// </code>
    /// </example>
    public static MessageBoxBuilder Input(this IMessageBoxService service, string message, string placeholder, string? title = null)
    {
        return MessageBoxBuilder.Create(service)
            .WithTitle(title ?? "Input")
            .WithMessage(message)
            .WithIcon(MessageBoxIcon.Question)
            .WithButtons(MessageBoxButtons.OKCancel)
            .WithInput(placeholder);
    }
}
