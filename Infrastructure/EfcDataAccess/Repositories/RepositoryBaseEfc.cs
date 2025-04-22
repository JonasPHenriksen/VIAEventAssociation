using DCAExamples.Core.Domain.Common.Repositories;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Common.Bases;

namespace EfcDataAccess.Repositories;

public abstract class RepositoryBaseEfc<T>(DbContext context) 
    : IGenericRepository<T> 
    where T : AggregateRoot
{
    public virtual async Task<T> GetAsync(Guid id) //TODO we could use the base and give them Guid instead of direct value objects
    {
        T? root = await context.Set<T>().FindAsync(id);
        return root!;
    }
    public virtual async Task AddAsync(T aggregate)
        => await context.Set<T>().AddAsync(aggregate);
    
    public virtual async Task RemoveAsync(Guid id)
    {
        T? root = await context.Set<T>().FindAsync(id);
        context.Set<T>().Remove(root!);
    }
}