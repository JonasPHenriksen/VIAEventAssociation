using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using QueryContracts.Contract;
using QueryContracts.Queries;
using VIAEventAssociation.Core.Domain.Contracts;

namespace EfcQueries;

public class UpcomingEventsQueryHandler(VeadatabaseProductionContext context, ISystemTime time) : IQueryHandler<UpcomingEvents.Query, UpcomingEvents.Answer>
{
    public async Task<UpcomingEvents.Answer> HandleAsync(UpcomingEvents.Query query)
    {
        var now = time.Now;

        var events = await context.Events
            .Where(e => e.Status == 1 && e.StartTime != null)
            .ToListAsync();

        var upcoming = events
            .Where(e => DateTime.TryParse(e.StartTime, out var startTime) && startTime > now)
            .OrderBy(e => DateTime.Parse(e.StartTime))
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var result = new List<UpcomingEvents.UpcomingEvent>();

        foreach (var e in upcoming)
        {
            var acceptedInvitations = await context.Invitations
                .Where(i => i.VeaEventId == e.EventId && i.Status == "Accepted")
                .Select(i => i.GuestId)
                .ToListAsync();

            var acceptedParticipants = await context.GuestIds
                .Where(i => i.VeaEventEventId == e.EventId)
                .Select(i => i.Value)
                .ToListAsync();

            var total = acceptedInvitations.Count + acceptedParticipants.Count;

            result.Add(new UpcomingEvents.UpcomingEvent(
                e.Title,
                e.Description.Length > 50 ? e.Description[..50] : e.Description,
                e.MaxGuests,
                total,
                e.Visibility.ToString(),
                e.StartTime,
                e.EndTime));
        }

        return new UpcomingEvents.Answer(result);
    }
}
