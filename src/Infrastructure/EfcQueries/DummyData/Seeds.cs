using Microsoft.EntityFrameworkCore;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace EfcDataAccess.Context;

public class Seeds
{
    public static VeadatabaseProductionContext Seed(VeadatabaseProductionContext context)
    {
        context.Guests.AddRange(GuestSeedFactory.CreateGuests());
        List<Event> veaEvents = EventSeedFactory.CreateEvents();
        context.Events.AddRange(veaEvents);
        context.SaveChanges();
        ParticipationSeedFactory.Seed(context);
        context.SaveChanges();
        InvitationSeedFactory.Seed(context);
        context.SaveChanges();
        return context;
    }

    public static VeadatabaseProductionContext SetupReadContext(bool production) //TODO for test purpose ONLY
    {
        DbContextOptionsBuilder<VeadatabaseProductionContext> optionsBuilder = new();
        string connString =
            @"Data Source = /home/jonas/RiderProjects/VIAEventAssociation/Infrastructure/EfcDataAccess/VEADatabaseProduction.db";
        if (!production)
        {
            connString += "@\"Data Source = Test" + Guid.NewGuid() + ".db";
        }
        optionsBuilder.UseSqlite(connString);
        VeadatabaseProductionContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
    
}