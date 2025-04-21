using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;

namespace VIAEventAssociation.Core.Domain.Common.Contracts;

public interface IGuestRepository //TODO: These will be implemented in the infrastructure layer later on
{
    Task<Guest?> GetByEmailAsync(Email email);
    Task<Guest?> GetByGuestIdAsync(GuestId id);
    Task AddAsync(Guest guest);
    Task RemoveAsync(GuestId id);
}
