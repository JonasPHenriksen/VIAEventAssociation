using DCAExamples.Core.Domain.Common.Repositories;
using EfcDataAccess.Context;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Common.Contracts;

namespace EfcDataAccess.Repositories;

public class GuestRepositoryEfc (MyDbContext context) : RepositoryBaseEfc<Guest> (context), IGuestRepository
{
    public async Task<Guest?> GetByGuestIdAsync(GuestId guestId)
    {
        return await context.Set<Guest>().FindAsync(guestId);
    }
    public async Task<Guest?> GetByEmailAsync(Email email)
    {
        return await context.Set<Guest>().SingleOrDefaultAsync(g => g.Email == email);
    }
    
    public async Task RemoveAsync(GuestId id)
    {
        var guestToDelete = await context.Set<Guest>().FindAsync(id);
        if (guestToDelete != null)
        {
            context.Set<Guest>().Remove(guestToDelete);
        }
    }
    
}