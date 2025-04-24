using VIAEventAssociation.Core.Domain.Common.Bases;

namespace DCAExamples.Core.Domain.Common.Repositories;

public interface IGenericRepository<T, TId>
{
    Task<T?> GetAsync(TId id);
    Task AddAsync(T aggregate);
    Task RemoveAsync(TId id);
}
