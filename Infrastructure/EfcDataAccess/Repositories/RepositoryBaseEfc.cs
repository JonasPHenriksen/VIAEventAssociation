using DCAExamples.Core.Domain.Common.Repositories;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Common.Bases;

namespace EfcDataAccess.Repositories;

public abstract class RepositoryBaseEfc<TAgg, TId>(DbContext context) :
    IGenericRepository<TAgg, TId>
    where TAgg : AggregateRoot
{
    public virtual async Task<TAgg> GetAsync(TId id)
    {
        TAgg? root = await context.Set<TAgg>().FindAsync(id);
        return root!;
    }
    public virtual async Task RemoveAsync(TId id)
    {
        TAgg? root = await context.Set<TAgg>().FindAsync(id);
        context.Set<TAgg>().Remove(root!);
    }
    public virtual async Task AddAsync(TAgg aggregate)
        => await context.Set<TAgg>().AddAsync(aggregate);
}

