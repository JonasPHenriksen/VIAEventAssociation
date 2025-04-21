using EfcDataAccess.Context;
using EfcDataAccess.Repositories;
using EfcMappingExamples;
using Microsoft.EntityFrameworkCore;
using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit.Abstractions;

namespace IntegrationTests.Repositories;

public class EventRepositoryEfcTests
{
    private const string EmailAddress = "330943@via.dk";
    private const string FirstName = "John";
    private const string LastName = "Doe";
    private const string ProfilePictureUrl = "https://example.com/profile.jpg";
    private static readonly Uri ValidProfilePictureUrl = new Uri(ProfilePictureUrl);
    private readonly DbLogger _dblogger;
    public EventRepositoryEfcTests(ITestOutputHelper output)
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
    public async Task AddAndGet_EventWithParticipantsAndInvitations_ShouldBeSame()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest = GetGuest();
        var guest2 = GetGuest();

        await guestRepo.AddAsync(guest);
        await guestRepo.AddAsync(guest2);
        await uow.SaveChangesAsync();  

        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithTimeRange(DateTime.Now.AddDays(5), DateTime.Now.AddDays(5).AddHours(2))
            .Build();
        
        //veaEvent.Participate(guest.GuestId);
        veaEvent.InviteGuest(guest2.GuestId);
        
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        await _dblogger.LogDatabaseContentsAsync(context);
          
        context.ChangeTracker.Clear();
        
        var loaded = await eventRepo.GetAsync(veaEvent.Id);
        //var participants = context.Entry(loaded!).Collection("Participants");
        var invitations = context.Entry(loaded!).Collection("_invitations");

        //await participants.LoadAsync();
        await invitations.LoadAsync();

        Assert.Equal(veaEvent.Id, loaded.Id);
        //Assert.Single(context.Entry(loaded).Collection("Participants").CurrentValue as IEnumerable<object>);
        Assert.Single(context.Entry(loaded).Collection("_invitations").CurrentValue as IEnumerable<object>);

        await eventRepo.RemoveAsync(loaded.Id);
        await uow.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var loaded2 = await eventRepo.GetAsync(veaEvent.Id);
        
        Assert.Equal(loaded2, null);
    }
}