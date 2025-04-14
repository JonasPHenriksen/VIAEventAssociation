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
    public override async Task<Guest?> GetAsync(Guid id)
    {
        return await context.Set<Guest>().FindAsync(id);
    }

    public async Task<Guest?> GetByGuestIdAsync(GuestId guestId)
    {
        return await context.Set<Guest>().FindAsync(guestId.Value);
    }
    public async Task<Guest?> GetByEmailAsync(Email email)
    {
        throw new InvalidOperationException("An unhandled exception occurred.");
        //return await context.Set<Guest>().SingleOrDefaultAsync(g => g.Email == email.Value);
    }
    

    public async Task UpdateAsync(Guest guest)
    {
        context.Set<Guest>().Update(guest);
    }
}