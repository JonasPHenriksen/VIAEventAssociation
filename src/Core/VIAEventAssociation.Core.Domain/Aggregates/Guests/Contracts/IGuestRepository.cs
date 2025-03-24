using VIAEventAssociation.Core.Domain.Aggregates.Guests;

namespace VIAEventAssociation.Core.Domain.Common.Contracts;

public interface IGuestRepository
{
    Task<Guest?> GetByEmailAsync(Email email);
    Task<Guest?> GetByIdAsync(GuestId id);
    Task AddAsync(Guest guest);
}
