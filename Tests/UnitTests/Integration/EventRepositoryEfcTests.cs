using EfcDataAccess.Context;
using Xunit;
using Microsoft.EntityFrameworkCore;
using EfcMappingExamples;
using EfcDataAccess.Repositories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Contracts;

public class EventRepositoryEfcTests
{
    private MyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }

    [Fact]
    public async Task SaveAndLoad_Event_ShouldMatch()
    {
        var context = CreateDbContext();
        var unitOfWork = new SqliteUnitOfWork(context);
        var repo = new EventRepositoryEfc(context);

        var veaEvent = VeaEvent.Create();

        await repo.AddAsync(veaEvent.Value);
        await unitOfWork.SaveChangesAsync();

        context = CreateDbContext(); // clear context
        repo = new EventRepositoryEfc(context);

        var loaded = await repo.GetAsync(veaEvent.Value.Id);
        Assert.Equal(veaEvent.Value.Id, loaded?.Id);
        // Add more assertions for nested entities / value objects
    }
}