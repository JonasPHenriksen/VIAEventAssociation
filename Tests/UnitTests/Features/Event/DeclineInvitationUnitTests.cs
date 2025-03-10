using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class DeclineInvitationUnitTests
{
    
    [Fact]
    public void DeclineInvitation_Success_WhenGuestHasPendingInvitation()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.CreateGuest();
        newEvent.InviteGuest(guest.GuestId);

        var result = newEvent.DeclineInvitation(guest.GuestId);

        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.GetDeclinedGuests());
        Assert.DoesNotContain(guest.GuestId, newEvent.GetInvitedGuests());
    }

    [Fact]
    public void DeclineInvitation_Success_WhenGuestHasAcceptedInvitation()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.CreateGuest();
        var result1 = newEvent.InviteGuest(guest.GuestId);
        var result2 = newEvent.AcceptInvitation(guest.GuestId);

        var result = newEvent.DeclineInvitation(guest.GuestId);

        Assert.True(result.IsSuccess);
        Assert.Contains(guest.GuestId, newEvent.GetDeclinedGuests());
        Assert.DoesNotContain(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void DeclineInvitation_Fails_WhenInvitationNotFound()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.CreateGuest();

        var result = newEvent.DeclineInvitation(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationNotFound", result.Errors.First().Code);
    }

    [Fact]
    public void DeclineInvitation_Fails_WhenEventIsCancelled()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guest = GuestFactory.CreateGuest();
        newEvent.InviteGuest(guest.GuestId);

        newEvent.Status = EventStatus.Cancelled;
        
        var result = newEvent.DeclineInvitation(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationDeclineCancel", result.Errors.First().Code);
    }
}
