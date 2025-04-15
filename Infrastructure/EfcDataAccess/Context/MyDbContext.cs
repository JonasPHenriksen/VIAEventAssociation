using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;


namespace EfcDataAccess.Context;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    
    public DbSet<VeaEvent> Events => Set<VeaEvent>();
    public DbSet<Guest> Guests => Set<Guest>();
    
    public MyDbContext() : this(new DbContextOptions<MyDbContext>())
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(@"Data Source = Test.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GuestEntityConfiguration());
    }
    
    public static MyDbContext SetupContext()
    {
        DbContextOptionsBuilder<MyDbContext> optionsBuilder = new();
        string testDbName = "Test" + Guid.NewGuid() +".db";
        optionsBuilder.UseSqlite(@"Data Source = " + testDbName);
        MyDbContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }
    
    private static async Task SaveAndClearAsync<T>(T entity, MyDbContext context) 
        where T : class
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
    
}

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        optionsBuilder.UseSqlite(@"Data Source = VEADatabaseProduction.db");
        return new MyDbContext(optionsBuilder.Options);
    }
}
