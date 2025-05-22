using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using QueryContracts.Contract;
using QueryContracts.QueryDispatching;

public static class QueryHandlerRegistrationExtensions
{
    public static IServiceCollection RegisterQueryHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlers = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(t => t.iface.IsGenericType && t.iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.iface, handler.type);
        }

        return services;
    }

    public static IServiceCollection RegisterQueryDispatching(this IServiceCollection services)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        return services;
    }
}
