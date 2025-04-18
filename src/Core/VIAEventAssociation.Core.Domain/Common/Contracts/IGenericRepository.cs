﻿using VIAEventAssociation.Core.Domain.Common.Bases;

namespace DCAExamples.Core.Domain.Common.Repositories;

public interface IGenericRepository<T>
    where T : AggregateRoot
{
    Task<T> GetAsync(Guid id);
    Task AddAsync(T aggregate);
    Task RemoveAsync(Guid id);
}
