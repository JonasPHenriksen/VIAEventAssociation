using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

public class InviteGuestUnitTests
{
    [Fact]
    public void InviteGuest_Success_WhenEventIsReady()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Ready)
            .Build();

        var guestId = GuestFactory.Init().Build().Value.GuestId;

        // Act
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.GetInvitedGuests());
    }

    [Fact]
    public void InviteGuest_Success_WhenEventIsActive()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guestId = GuestFactory.Init().Build().Value.GuestId;

        // Act
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.GetInvitedGuests());
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsDraft()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Draft)
            .Build();

        var guestId = GuestFactory.Init().Build().Value.GuestId;

        // Act
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Cancelled)
            .Build();

        var guestId = GuestFactory.Init().Build().Value.GuestId;

        // Act
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsFull()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(5)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        for (int i = 0; i < 5; i++)
        {
            newEvent.InviteGuest(GuestFactory.Init().Build().Value.GuestId);
        }

        // Act
        var guestId = GuestFactory.Init().Build().Value.GuestId;
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenGuestIsAlreadyInvited()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();
        
        var guestId = GuestFactory.Init().Build().Value.GuestId;
        newEvent.InviteGuest(guestId);

        // Act
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateInvitation", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenGuestIsAlreadyParticipating()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guestId = GuestFactory.Init().Build().Value.GuestId;
        newEvent.Participate(guestId);

        // Act
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("GuestAlreadyParticipate", result.Errors.First().Code);
    }
}
