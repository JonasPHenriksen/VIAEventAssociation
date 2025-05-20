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
    public async Task AddGuest_ShouldAddGuestToRepository()
    {
        using var context = MyDbContext.SetupContext();
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest = GetGuest();
        await guestRepo.AddAsync(guest);
        await uow.SaveChangesAsync();

        var loadedGuest = await guestRepo.GetAsync(guest.GuestId);
        Assert.NotNull(loadedGuest);
    }

    [Fact]
    public async Task CreateEvent_ShouldSaveEventToRepository()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithTimeRange(DateTime.Now.AddDays(5), DateTime.Now.AddDays(5).AddHours(2))
            .Build();

        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        Assert.NotNull(loadedEvent);
    }

    [Fact]
    public async Task AddParticipantToEvent_ShouldAddParticipant()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest = GetGuest();
        await guestRepo.AddAsync(guest);
        await uow.SaveChangesAsync();

        var veaEvent = EventFactory.Init().WithStatus(EventStatus.Active).WithVisibility(EventVisibility.Public).WithTimeRange(DateTime.Now.AddDays(1),DateTime.Now.AddDays(1).AddHours(1)).Build(); //TODO Probably change the Creator to take EventTimeRangeObject
        veaEvent.Participate(guest.GuestId);
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();
        
        context.ChangeTracker.Clear();

        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        var participants = context.Entry(loadedEvent!).Collection("Participants");
        await participants.LoadAsync();

        Assert.Single(participants.CurrentValue as IEnumerable<object>);
    }

    [Fact]
    public async Task InviteGuestToEvent_ShouldAddInvitation()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest = GetGuest();
        await guestRepo.AddAsync(guest);
        await uow.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var veaEvent = EventFactory.Init().WithStatus(EventStatus.Ready).Build();
        var repoGuest = await guestRepo.GetByEmailAsync(guest.Email);
        veaEvent.InviteGuest(repoGuest.GuestId);
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        await context.Entry(loadedEvent!).Collection("_invitations").LoadAsync();

        Assert.Single(context.Entry(loadedEvent).Collection("_invitations").CurrentValue as IEnumerable<object>);
    }

    [Fact]
    public async Task RetrieveEvent_ShouldReturnCorrectEvent()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var veaEvent = EventFactory.Init().Build();
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        Assert.Equal(veaEvent.Id, loadedEvent.Id);
    }
    
        [Fact]
    public async Task RemoveEvent_ShouldDeleteEventFromRepository()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var veaEvent = EventFactory.Init().Build();
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        // Remove the event
        await eventRepo.RemoveAsync(veaEvent.EventId);
        await uow.SaveChangesAsync();

        // Verify that the event has been removed
        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        Assert.Null(loadedEvent);
    }

    [Fact]
    public async Task EventParticipantsCount_ShouldMatchExpectedCount()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest1 = GetGuest();
        var guest2 = GetGuest();
        await guestRepo.AddAsync(guest1);
        await guestRepo.AddAsync(guest2);
        await uow.SaveChangesAsync();

        var veaEvent = EventFactory.Init().WithStatus(EventStatus.Active).WithVisibility(EventVisibility.Public).WithTimeRange(DateTime.Now.AddDays(1),DateTime.Now.AddDays(1).AddHours(1)).Build();
        veaEvent.Participate(guest1.GuestId);
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        context.ChangeTracker.Clear();
        
        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        var participants = context.Entry(loadedEvent!).Collection("Participants");
        await participants.LoadAsync();

        Assert.Single(participants.CurrentValue as IEnumerable<object>);
    }

    [Fact]
    public async Task EventInvitationsCount_ShouldMatchExpectedCount()
    {
        using var context = MyDbContext.SetupContext();
        var eventRepo = new EventRepositoryEfc(context);
        var guestRepo = new GuestRepositoryEfc(context);
        var uow = new SqliteUnitOfWork(context);

        var guest1 = GetGuest();
        var guest2 = GetGuest();
        await guestRepo.AddAsync(guest1);
        await guestRepo.AddAsync(guest2);
        await uow.SaveChangesAsync();
        
        var veaEvent = EventFactory.Init().WithStatus(EventStatus.Ready).Build();
        veaEvent.InviteGuest(guest2.GuestId);
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        context.ChangeTracker.Clear();
        
        var loadedEvent = await eventRepo.GetAsync(veaEvent.EventId);
        var invitations = context.Entry(loadedEvent!).Collection("_invitations");
        await invitations.LoadAsync();

        Assert.Single(invitations.CurrentValue as IEnumerable<object>);
    }
    
    [Fact]
    public async Task CombinedInitCreationOfEventWithGuests()
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
        
        veaEvent.Participate(guest.GuestId);
        veaEvent.InviteGuest(guest2.GuestId);
        await eventRepo.AddAsync(veaEvent);
        await uow.SaveChangesAsync();

        await _dblogger.LogDatabaseContentsAsync(context);
          
        context.ChangeTracker.Clear();
        
        var loaded = await eventRepo.GetAsync(veaEvent.EventId);
        var participants = context.Entry(loaded!).Collection("Participants");
        var invitations = context.Entry(loaded!).Collection("_invitations");

        await participants.LoadAsync();
        await invitations.LoadAsync();

        Assert.Equal(veaEvent.Id, loaded.Id);
        Assert.Single(context.Entry(loaded).Collection("Participants").CurrentValue as IEnumerable<object>);
        Assert.Single(context.Entry(loaded).Collection("_invitations").CurrentValue as IEnumerable<object>); //TODO split up these test and make them more isolated

        await eventRepo.RemoveAsync(loaded.EventId);
        await uow.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var loaded2 = await eventRepo.GetAsync(veaEvent.EventId);
        
        Assert.Equal(loaded2, null);
    }
}