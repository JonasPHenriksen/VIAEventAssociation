using UnitTests.Common;
using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class AcceptInvitationUnitTests
{
    [Fact]
    public void AcceptInvitation_Success_WhenEventIsActiveAndGuestIsInvited()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        
        var inviteResult = newEvent.InviteGuest(guest.GuestId);

        // Act
        var result = newEvent.AcceptInvitation(guest.GuestId);

        // Assert
        Assert.True(inviteResult.IsSuccess);
        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.GetParticipants());
        Assert.DoesNotContain(guest.GuestId, newEvent.GetInvitedGuests());
    }
    
    [Fact]
    public void AcceptInvitation_Fails_WhenGuestIsNotInvited()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value; // Guest who was never invited

        // Act
        var result = newEvent.AcceptInvitation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationNotFound", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventIsFull()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithMaxGuests(6)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithVisibility(EventVisibility.Public)
            .Build();
        
        var guests = Enumerable.Range(0, 6)
            .Select(_ => GuestFactory.Init().Build().Value)
            .ToList();

        foreach (var guest in guests)
            newEvent.InviteGuest(guest.GuestId);

        for (int i = 0; i < guests.Count - 1; i++)
            newEvent.AcceptInvitation(guests[i].GuestId);

        newEvent.MaxGuests = 5;
        
        var lastGuest = guests.Last();
        
        // Act
        
        var result = newEvent.AcceptInvitation(lastGuest.GuestId);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }
    
    [Fact]
    public void AcceptInvitation_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();
        
        var guest = GuestFactory.Init().Build().Value;
        newEvent.InviteGuest(guest.GuestId);

        newEvent.Status = EventStatus.Cancelled;

        // Act
        var result = newEvent.AcceptInvitation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventIsReady()
    {
        // Arrange
        var pastDate = DateTime.Now.AddYears(-1);
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(pastDate, pastDate.AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.InviteGuest(guest.GuestId);

        newEvent.Status = EventStatus.Ready;
        
        // Act
        var result = newEvent.AcceptInvitation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventHasStarted()
    {
        // Arrange
        var pastDate = DateTime.Now.AddYears(-1);
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(5)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        var result2 = newEvent.InviteGuest(guest.GuestId);
        
        newEvent.TimeRange = new EventTimeRange(pastDate, pastDate.AddHours(4));
        
        // Act
        var result = newEvent.AcceptInvitation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationNotFound", result.Errors.First().Code);
    }


}
