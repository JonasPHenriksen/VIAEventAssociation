using System.Reflection;
using AppEntry;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CommandDispatcher;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlers = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(t => t.iface.IsGenericType && t.iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

        foreach (var handler in handlers)
        {
            services.AddTransient(handler.iface, handler.type);
        }

        return services;
    }
}

// In application startup
// services.AddCommandHandlers(Assembly.GetExecutingAssembly()); OR
// services.AddCommandHandlers(typeof(CreateGuestCommandHandler).Assembly);