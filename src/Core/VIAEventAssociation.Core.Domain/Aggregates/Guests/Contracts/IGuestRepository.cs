using DCAExamples.Core.Domain.Common.Repositories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;

namespace VIAEventAssociation.Core.Domain.Common.Contracts;

public interface IGuestRepository : IGenericRepository<Guest, GuestId>
{
    Task<Guest?> GetByEmailAsync(Email email);
    
    //TODO: These will be implemented in the infrastructure layer later on
}
