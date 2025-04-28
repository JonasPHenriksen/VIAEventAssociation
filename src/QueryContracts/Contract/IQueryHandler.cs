namespace QueryContracts.Contract;

public interface IQueryHandler<in TQuery, TAnswer> where TQuery : IQuery.IQuery<TAnswer>
{
    public Task<TAnswer> HandleAsync(TQuery query);
}