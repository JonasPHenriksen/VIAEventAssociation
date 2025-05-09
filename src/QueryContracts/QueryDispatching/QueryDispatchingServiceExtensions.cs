using Microsoft.Extensions.DependencyInjection;
using QueryContracts.QueryDispatching;

namespace QueryContracts;


public static class QueryDispatchingServiceExtensions
{
    public static IServiceCollection RegisterQueryDispatching(this IServiceCollection services)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        return services;
    }
}