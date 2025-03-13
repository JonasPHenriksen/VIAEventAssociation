using UnitTests.Common;
using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class ParticipateInEventUnitTests
{
    [Fact]
    public void Participate_Success_WhenEventIsActiveAndPublic()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        var result = newEvent.Participate(guest.GuestId);

        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.Participants);
    }

    [Fact]
    public void Participate_Fails_WhenEventIsDraft()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        var result = newEvent.Participate(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventIsReady()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Ready)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        var result = newEvent.Participate(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventIsCancelled()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        var result = newEvent.Participate(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventIsFull()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(5)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guests = Enumerable.Range(0, 5).Select(_ => GuestFactory.Init().Build().Value).ToList();
        guests.ForEach(g => newEvent.Participate(g.GuestId));

        var extraGuest = GuestFactory.Init().Build().Value;
        var result = newEvent.Participate(extraGuest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventHasStarted()
    {
        var pastDate = DateTime.Now.AddYears(-1);
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(pastDate, pastDate.AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        var result = newEvent.Participate(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenEventIsPrivate()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .Build();
        
        var guest = GuestFactory.Init().Build().Value;

        var result = newEvent.Participate(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidVisibility", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenGuestIsAlreadyParticipating()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.Participate(guest.GuestId);

        var result = newEvent.Participate(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateParticipation", result.Errors.First().Code);
    }
}
