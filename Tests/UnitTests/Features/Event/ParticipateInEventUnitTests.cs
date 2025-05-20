using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class ParticipateInEventUnitTests
{
    [Fact]
    public void Participate_Success_WhenEventIsActiveAndPublic()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        // Act
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void Participate_Fails_WhenEventIsDraft()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        // Act
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventIsReady()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Ready)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        // Act
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        // Act
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventIsFull()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(5)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guests = Enumerable.Range(0, 5).Select(_ => GuestFactory.Init().Build().Value).ToList();
        guests.ForEach(g => newEvent.Participate(g.GuestId));

        // Act
        var extraGuest = GuestFactory.Init().Build().Value;
        var result = newEvent.Participate(extraGuest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }
    
    [Fact]
    public void Participate_Fails_WhenEventHasStarted()
    {
        //Arrange
        var pastDate = DateTime.Now.AddYears(-1);
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(pastDate, pastDate.AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        // Act
        var guest = GuestFactory.Init().Build().Value;
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenEventIsPrivate()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .Build();
        
        var guest = GuestFactory.Init().Build().Value;

        // Act
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidVisibility", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenGuestIsAlreadyParticipating()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.Participate(guest.GuestId);

        // Act
        var result = newEvent.Participate(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateParticipation", result.Errors.First().Code);
    }
}
