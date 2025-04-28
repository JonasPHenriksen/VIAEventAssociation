using QueryContracts.Contract;

namespace QueryContracts.QueryDispatching;

public interface IQueryDispatcher
{
    Task<TAnswer> DispatchAsync<TAnswer>(IQuery.IQuery<TAnswer> query);
}