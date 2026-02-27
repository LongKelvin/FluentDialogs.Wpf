using System;
using FluentDialogs.Abstractions;
using FluentDialogs.Models;
using FluentDialogs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDialogs;

/// <summary>
/// Extension methods for registering FluentDialogs services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds FluentDialogs services with the v2 theming system.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for <see cref="FluentDialogOptions"/>.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddFluentDialogs(options =>
    /// {
    ///     options.DefaultPreset = MessageBoxTheme.Dark;
    ///     options.AccentColor = Colors.Purple;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddFluentDialogs(this IServiceCollection services, Action<FluentDialogOptions>? configure = null)
    {
        var options = new FluentDialogOptions();
        configure?.Invoke(options);

        // v2 theme service (primary)
        services.AddSingleton(options);
        services.AddSingleton<IFluentDialogThemeService, FluentDialogThemeService>();

        // v1 legacy adapter — allows existing code using IMessageBoxThemeService to keep working
#pragma warning disable CS0618 // Obsolete
        services.AddSingleton<IMessageBoxThemeService>(sp =>
            new LegacyThemeServiceAdapter(sp.GetRequiredService<IFluentDialogThemeService>()));
#pragma warning restore CS0618

        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IToastService, ToastService>();

        return services;
    }

    /// <summary>
    /// Adds FluentDialogs services without theme support.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    /// Use this when you want to manage themes manually or don't need theme switching.
    /// </remarks>
    public static IServiceCollection AddFluentDialogsWithoutTheme(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBoxService>(sp => new MessageBoxService());

        return services;
    }
}
