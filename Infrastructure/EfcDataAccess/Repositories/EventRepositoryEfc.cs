using DCAExamples.Core.Domain.Common.Repositories;
using EfcDataAccess.Context;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Contracts;

namespace EfcDataAccess.Repositories;

public class EventRepositoryEfc (MyDbContext context) : RepositoryBaseEfc<VeaEvent> (context), IEventRepository
{
    public override async Task<VeaEvent?> GetAsync(Guid id)
    {
        return await context.Set<VeaEvent>().FindAsync(id);
    }
    
    public async Task<VeaEvent?> GetByIdAsync(EventId id)
    {
        return await context.Set<VeaEvent>().FindAsync(id);
    }
    
    public async Task DeleteAsync(EventId id)
    {
        var eventToDelete = await context.Set<VeaEvent>().FindAsync(id);
        if (eventToDelete != null)
        {
            context.Set<VeaEvent>().Remove(eventToDelete);
        }
    }

    public async Task<VeaEvent?> GetByGuestIdAsync(VeaEvent veaEvent)
    {
        return await context.Set<VeaEvent>().FindAsync(veaEvent.Id);
    }

    public async Task UpdateAsync(VeaEvent guest)
    {
        context.Set<VeaEvent>().Update(guest);
    }
}