using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using QueryContracts.Contract;
using QueryContracts.Queries;

namespace EfcQueries;

public class UpcomingEventsQueryHandler(VeadatabaseProductionContext context) : IQueryHandler<UpcomingEvents.Query, UpcomingEvents.Answer>
{
    public async Task<UpcomingEvents.Answer> HandleAsync(UpcomingEvents.Query query)
    {
        var currentDateTime = DateTime.UtcNow;
        
        var upcomingEvents = await context.Events
            .Where(e => e.Status == 1 && e.StartTime != null)
            .ToListAsync();

        var filteredEvents = upcomingEvents
            .Where(e => DateTime.TryParse(e.StartTime, out var startTime) && startTime > currentDateTime)
            .Select(e => new UpcomingEvents.UpcomingEvent(
                e.Title,
                e.Description.Length > 50 ? e.Description.Substring(0, 50) : e.Description,
                e.MaxGuests,
                e.Visibility.ToString(),
                e.StartTime,
                e.EndTime))
            .ToList();

        return new UpcomingEvents.Answer(filteredEvents);
    }
}