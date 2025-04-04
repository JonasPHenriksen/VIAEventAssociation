using DCAExamples.Core.Domain.Common.Repositories;
using EfcMappingExamples;
using VIAEventAssociation.Core.Domain.Common.Bases;

namespace EfcDataAccess.Repositories;

public abstract class RepositoryBase<T>(MyDbContext context) :
    IGenericRepository<T> where T : AggregateRoot
{
    public abstract Task<T> GetAsync(Guid id);

    public async Task RemoveAsync(Guid id)
    {
        T agg = await GetAsync(id);
        context.Set<T>().Remove(agg);
    }

    public async Task AddAsync(T aggregate)
        => await context.Set<T>().AddAsync(aggregate);
}