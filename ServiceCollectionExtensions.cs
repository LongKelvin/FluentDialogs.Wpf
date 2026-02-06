using FluentDialogs.Abstractions;
using FluentDialogs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDialogs;

/// <summary>
/// Extension methods for registering FluentDialogs services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds FluentDialogs services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    /// This registers the following services:
    /// <list type="bullet">
    /// <item><see cref="IMessageBoxService"/> - Modal dialog service</item>
    /// <item><see cref="IMessageBoxThemeService"/> - Theme management service</item>
    /// <item><see cref="IToastService"/> - Non-modal toast notifications</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var services = new ServiceCollection();
    /// services.AddFluentDialogs();
    /// </code>
    /// </example>
    public static IServiceCollection AddFluentDialogs(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBoxThemeService, MessageBoxThemeService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IToastService, ToastService>();

        return services;
    }

    /// <summary>
    /// Adds FluentDialogs services to the specified <see cref="IServiceCollection"/> without theme support.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    /// Use this method when you want to manage themes manually or don't need theme switching.
    /// </remarks>
    public static IServiceCollection AddFluentDialogsWithoutTheme(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBoxService>(sp => new MessageBoxService());

        return services;
    }
}
