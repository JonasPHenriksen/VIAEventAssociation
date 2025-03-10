using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

public class InviteGuestUnitTests
{
    //.WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))

    [Fact]
    public void InviteGuest_Success_WhenEventIsReady()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Ready)
            .Build();

        var guestId = GuestFactory.CreateGuest().GuestId;

        var result = newEvent.InviteGuest(guestId);

        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.GetInvitedGuests());
    }

    [Fact]
    public void InviteGuest_Success_WhenEventIsActive()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guestId = GuestFactory.CreateGuest().GuestId;

        var result = newEvent.InviteGuest(guestId);

        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.GetInvitedGuests());
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsDraft()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Draft)
            .Build();

        var guestId = GuestFactory.CreateGuest().GuestId;

        var result = newEvent.InviteGuest(guestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsCancelled()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Cancelled)
            .Build();

        var guestId = GuestFactory.CreateGuest().GuestId;

        var result = newEvent.InviteGuest(guestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsFull()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(5)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        for (int i = 0; i < 5; i++)
        {
            newEvent.InviteGuest(GuestFactory.CreateGuest().GuestId);
        }

        var guestId = GuestFactory.CreateGuest().GuestId;
        var result = newEvent.InviteGuest(guestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenGuestIsAlreadyInvited()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guestId = GuestFactory.CreateGuest().GuestId;
        newEvent.InviteGuest(guestId);

        var result = newEvent.InviteGuest(guestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateInvitation", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenGuestIsAlreadyParticipating()
    {
        var newEvent = EventFactory.Init()
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Active)
            .Build();

        var guestId = GuestFactory.CreateGuest().GuestId;
        newEvent.Participate(guestId);

        var result = newEvent.InviteGuest(guestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("GuestAlreadyParticipate", result.Errors.First().Code);
    }
}
