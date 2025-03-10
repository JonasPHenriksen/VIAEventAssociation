using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace VIAEventAssociation.Core.Domain.Contracts;

public interface IInvitationRepository
{
    Task<Invitation?> GetByEventAndGuestAsync(EventId eventId, GuestId guestId);
    Task SaveAsync(Invitation invitation);
}