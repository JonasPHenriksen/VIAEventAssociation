using DCAExamples.Core.Domain.Common.Repositories;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;

namespace EfcDataAccess.Repositories;

public class TeamRepositoryEfc(MyDbContext context) 
                                : ITeamRepository
{
    public async Task AddAsync(Guest guest)
    {
        await context.Set<Guest>().AddAsync(guest);
        await context.SaveChangesAsync();
    }

    public Task<Guest> GetAsync(Guid id)
    {
        return context
            .Set<Guest>()
            //.Include(team => team.Members)
            .SingleAsync(team => team.Id == id);
    }
    
    // ...

    public Task RemoveAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guest guest)
    {
        throw new NotImplementedException();
    }
}