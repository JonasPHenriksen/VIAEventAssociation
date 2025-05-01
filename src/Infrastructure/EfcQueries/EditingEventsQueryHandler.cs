using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using QueryContracts.Contract;
using QueryContracts.Queries;

namespace EfcQueries;

public class EditingEventsQueryHandler (VeadatabaseProductionContext context) : IQueryHandler<EditingEvents.Query, EditingEvents.Answer>
{
    public async Task<EditingEvents.Answer> HandleAsync(EditingEvents.Query query)
    {
        var drafts = await context.Events
            .Where(e => e.Status == 0)
            .Select(e => new EditingEvents.EventItem(e.EventId, e.Title))
            .ToListAsync();

        var readied = await context.Events
            .Where(e => e.Status == 1)
            .Select(e => new EditingEvents.EventItem(e.EventId, e.Title))
            .ToListAsync();

        var cancelled = await context.Events
            .Where(e => e.Status == 2)
            .Select(e => new EditingEvents.EventItem(e.EventId, e.Title))
            .ToListAsync();
        
        return new EditingEvents.Answer(drafts, readied, cancelled);
    }
}