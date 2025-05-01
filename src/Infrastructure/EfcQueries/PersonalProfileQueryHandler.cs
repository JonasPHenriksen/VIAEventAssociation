using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using QueryContracts.Contract;
using QueryContracts.Queries;
using VIAEventAssociation.Core.Domain.Contracts;

namespace EfcQueries;

public class PersonalProfileQueryHandler(VeadatabaseProductionContext context, ISystemTime time)  : IQueryHandler<PersonalProfile.Query, PersonalProfile.Answer>
{
    public async Task<PersonalProfile.Answer> HandleAsync(PersonalProfile.Query query)
    {
        var guest = await context.Guests
            .FirstOrDefaultAsync(g => g.GuestId == query.GuestId);

        if (guest == null)
            throw new Exception("Guest not found");

        var now = time.Now;

        var events = await context.Events
            .Where(e => e.Status == 1 && e.StartTime != null)
            .ToListAsync();

        var invitations = await context.Invitations
            .Where(i => events.Select(e => e.EventId).Contains(i.VeaEventId))
            .ToListAsync();

        var acceptedInvitations = invitations
            .Where(i => i.Status == "Accepted")
            .GroupBy(i => i.VeaEventId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.GuestId).ToList());

        var pendingInvitations = invitations
            .Where(i => i.Status == "Pending")
            .GroupBy(i => i.VeaEventId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.GuestId).ToList());

        var acceptedParticipants = await context.GuestIds
            .Where(i => events.Select(e => e.EventId).Contains(i.VeaEventEventId))
            .GroupBy(i => i.VeaEventEventId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(i => i.Value).ToList());

        var upcomingEvents = new List<PersonalProfile.UpcomingEvent>();
        var pastEvents = new List<PersonalProfile.PastEvent>();

        int upcomingCount = 0;
        int pendingsCount = 0;

        foreach (var e in events)
        {
            var eventId = e.EventId;

            var totalAcceptedAttendees = acceptedInvitations.TryGetValue(eventId, out var accepted) ? accepted.Count : 0;
            totalAcceptedAttendees += acceptedParticipants.TryGetValue(eventId, out var participants) ? participants.Count : 0;

            if (acceptedInvitations.TryGetValue(eventId, out var acceptedGuests) && acceptedGuests.Contains(query.GuestId))
            {
                upcomingCount++;
            }

            if (pendingInvitations.TryGetValue(eventId, out var pendingGuests) && pendingGuests.Contains(query.GuestId))
            {
                pendingsCount++;
            }

            if (DateTime.TryParse(e.StartTime, out var startTime))
            {
                if (startTime > now)
                {
                    upcomingEvents.Add(new PersonalProfile.UpcomingEvent(e.Title, totalAcceptedAttendees, e.StartTime));
                }
                else
                {
                    pastEvents.Add(new PersonalProfile.PastEvent(e.Title));
                }
            }
        }

        return new PersonalProfile.Answer(
            $"{guest.FirstName} {guest.LastName}",
            guest.Email,
            guest.ProfilePictureUrl,
            upcomingCount,
            pendingsCount,
            upcomingEvents,
            pastEvents);
    }
}