using UnitTests.Common;
using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class CancelParticipationUnitTests
{
    [Fact]
    public void CancelParticipation_Success_WhenGuestIsParticipating()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.Participate(guest.GuestId);
        var result = newEvent.CancelParticipation(guest.GuestId);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void CancelParticipation_Success_WhenGuestIsNotParticipating()
    {
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;

        var result = newEvent.CancelParticipation(guest.GuestId);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void CancelParticipation_Fails_WhenEventHasStarted()
    {
        var pastDate = DateTime.Now.AddYears(-1);
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.Participate(guest.GuestId);

        newEvent.TimeRange = new EventTimeRange(pastDate,pastDate.AddHours(4));
        var result = newEvent.CancelParticipation(guest.GuestId);

        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }
}
