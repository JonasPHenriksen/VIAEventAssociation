using QueryContracts.Contract;

namespace QueryContracts.QueryDispatching;

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public Task<TAnswer> DispatchAsync<TAnswer>(IQuery.IQuery<TAnswer> query)
    {
        Type queryInterfaceWithTypes = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TAnswer));
        dynamic handler = serviceProvider.GetService(queryInterfaceWithTypes)!;
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for query type '{query.GetType()}' and answer type '{typeof(TAnswer)}'.");
        }
        return handler.HandleAsync((dynamic)query);
    }
}