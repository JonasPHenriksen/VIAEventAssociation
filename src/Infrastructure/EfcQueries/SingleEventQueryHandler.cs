using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using QueryContracts.Contract;
using QueryContracts.Queries;
using VIAEventAssociation.Core.Domain.Common.Contracts;

namespace EfcQueries;

public class SingleEventQueryHandler(VeadatabaseProductionContext context) : IQueryHandler<SingleEvent.Query, SingleEvent.Answer>
{
    public async Task<SingleEvent.Answer> HandleAsync(SingleEvent.Query query)
    {
        var eventEntity = await context.Events
            .FirstOrDefaultAsync(e => e.EventId == query.EventId);
        if (eventEntity == null)
            throw new Exception("Event not found");

        var acceptedInvitations = await context.Invitations
            .Where(i => i.VeaEventId == query.EventId && i.Status == "Accepted")
            .Select(i => i.GuestId)
            .ToListAsync();
        
        var acceptedParticipants = await context.GuestIds
            .Where(i => i.VeaEventEventId == query.EventId)
            .Select(i => i.Value)
            .ToListAsync();
        
        acceptedInvitations.AddRange(acceptedParticipants);
        
        var guests = await context.Guests
            .Where(g => acceptedInvitations.Contains(g.GuestId))
            .OrderBy(g => g.FirstName)
            .Select(g => new SingleEvent.Guest(
                g.GuestId,
                g.FirstName + " " + g.LastName,
                g.ProfilePictureUrl
            ))
            .ToListAsync();

        var guestCount = acceptedInvitations.Count;

        return new SingleEvent.Answer(
            eventEntity.Title,
            eventEntity.Description,
            eventEntity.StartTime,
            eventEntity.EndTime,
            eventEntity.Visibility.ToString(),
            guestCount,
            eventEntity.MaxGuests,
            guests);
    }
}