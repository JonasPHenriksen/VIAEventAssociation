using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class DeclineInvitationUnitTests
{
    
    [Fact]
    public void DeclineInvitation_Success_WhenGuestHasPendingInvitation()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.InviteGuest(guest.GuestId);

        // Act
        var result = newEvent.DeclineInvitation(guest.GuestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.GetDeclinedGuests());
        Assert.DoesNotContain(guest.GuestId, newEvent.GetInvitedGuests());
    }

    [Fact]
    public void DeclineInvitation_Success_WhenGuestHasAcceptedInvitation()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.InviteGuest(guest.GuestId);
        newEvent.AcceptInvitation(guest.GuestId);

        // Act
        var result = newEvent.DeclineInvitation(guest.GuestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.GetDeclinedGuests());
        Assert.DoesNotContain(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void DeclineInvitation_Fails_WhenInvitationNotFound()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        // Act
        var result = newEvent.DeclineInvitation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationNotFound", result.Errors.First().Code);
    }

    [Fact]
    public void DeclineInvitation_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.InviteGuest(guest.GuestId);

        newEvent.Status = EventStatus.Cancelled;
        
        // Act
        var result = newEvent.DeclineInvitation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationDeclineCancel", result.Errors.First().Code);
    }
}
