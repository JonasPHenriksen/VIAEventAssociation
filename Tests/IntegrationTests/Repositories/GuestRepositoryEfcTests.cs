using EfcDataAccess.Context;
using EfcDataAccess.Repositories;
using EfcMappingExamples;
using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Repositories;

public class GuestRepositoryEfcTests
{
    private const string EmailAddress = "330943@via.dk";
    private const string FirstName = "John";
    private const string LastName = "Doe";
    private const string ProfilePictureUrl = "https://example.com/profile.jpg";
    private static readonly Uri ValidProfilePictureUrl = new Uri(ProfilePictureUrl);
    private readonly DbLogger _dblogger;
    public GuestRepositoryEfcTests(ITestOutputHelper output)
    {
        _dblogger = new DbLogger(output);
    }
    
    private static Guest GetGuest()
    {
        var factory = GuestFactory.Init()
            .WithEmail(Email.Create(EmailAddress).Value)
            .WithFirstName(Name.Create(FirstName).Value)
            .WithLastName(Name.Create(LastName).Value)
            .WithProfilePicture(ValidProfilePictureUrl);
        
        return factory.Build().Value;
    }
    
    [Fact]
    public async Task AddAndGetGuest_ShouldReturnSameGuest()
    {
        using var context = MyDbContext.SetupContext();
        var repo = new GuestRepositoryEfc(context);
        var unitOfWork = new SqliteUnitOfWork(context);
        
        var guest = GetGuest();

        await repo.AddAsync(guest);
        await unitOfWork.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var loaded = await repo.GetAsync(guest.GuestId);
        Assert.Equal(guest.Id, loaded?.Id);
    }
    
    [Fact]
    public async Task AddAndGetByEmailGuest_ShouldReturnSameGuest()
    {
        using var context = MyDbContext.SetupContext();
        var repo = new GuestRepositoryEfc(context);
        var unitOfWork = new SqliteUnitOfWork(context);
        
        var guest = GetGuest();

        await repo.AddAsync(guest);
        await unitOfWork.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var loaded = await repo.GetByEmailAsync(Email.Create("330943@via.dk").Value);
        Assert.Equal(guest.Id, loaded?.Id);
    }
    
    [Fact]
    public async Task AddAndRemove_ShouldReturnNoGuest()
    {
        using var context = MyDbContext.SetupContext();
        var repo = new GuestRepositoryEfc(context);
        var unitOfWork = new SqliteUnitOfWork(context);
        
        var guest = GetGuest();

        await repo.AddAsync(guest);
        await unitOfWork.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var loaded = await repo.GetAsync(guest.GuestId);
        Assert.Equal(guest, loaded);
        
        await repo.RemoveAsync(guest.GuestId);
        await unitOfWork.SaveChangesAsync();

        var loaded2 = await repo.GetAsync(guest.GuestId);
        Assert.Equal(loaded2, null);
    }
    

}