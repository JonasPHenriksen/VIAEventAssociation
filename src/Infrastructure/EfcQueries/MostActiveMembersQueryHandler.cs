using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using QueryContracts.Contract;
using QueryContracts.Queries;
using VIAEventAssociation.Core.Domain.Contracts;

public class MostActiveMembersQueryHandler(VeadatabaseProductionContext context, ISystemTime time)
    : IQueryHandler<MostActiveMembers.Query, MostActiveMembers.Answer>
{
    public async Task<MostActiveMembers.Answer> HandleAsync(MostActiveMembers.Query query)
    {
        var now = time.Now;
        var sixMonthsAgo = now.AddMonths(-6);

        var events = await context.Events
            .Where(e => e.Status == 1 && e.StartTime != null)
            .ToListAsync();

        var eventMap = events.ToDictionary(e => e.EventId, e => DateTime.Parse(e.StartTime));
        var validEventIds = eventMap.Keys.ToHashSet();

        var invitations = await context.Invitations
            .Where(i => validEventIds.Contains(i.VeaEventId) && i.Status == "Accepted")
            .ToListAsync();

        var grouped = invitations
            .GroupBy(i => i.GuestId)
            .Select(g => new
            {
                GuestId = g.Key,
                Total = g.Count(),
                LastSixMonths = g.Count(i => eventMap[i.VeaEventId] >= sixMonthsAgo),
                Upcoming = g
                    .Where(i => eventMap[i.VeaEventId] > now)
                    .Select(i => events.First(e => e.EventId == i.VeaEventId).Title)
                    .Distinct()
                    .ToList()
            })
            .OrderByDescending(x => x.LastSixMonths)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var guests = await context.Guests
            .Where(g => grouped.Select(x => x.GuestId).Contains(g.GuestId))
            .ToListAsync();

        var members = grouped
            .Select(g =>
            {
                var guest = guests.First(x => x.GuestId == g.GuestId);
                return new MostActiveMembers.Member($"{guest.FirstName} {guest.LastName}", g.LastSixMonths, g.Total, g.Upcoming);
            })
            .ToList();

        return new MostActiveMembers.Answer(members);
    }
}

