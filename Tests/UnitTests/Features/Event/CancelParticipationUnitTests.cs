using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class CancelParticipationUnitTests
{
    [Fact]
    public void CancelParticipation_Success_WhenGuestIsParticipating()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        // Act
        var guest = GuestFactory.Init().Build().Value;
        newEvent.Participate(guest.GuestId);
        var result = newEvent.CancelParticipation(guest.GuestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void CancelParticipation_Success_WhenGuestIsNotParticipating()
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
        var result = newEvent.CancelParticipation(guest.GuestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(guest.GuestId, newEvent.GetParticipants());
    }

    [Fact]
    public void CancelParticipation_Fails_WhenEventHasStarted()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Parse("2020-01-01T23:30:00"), DateTime.Parse("2020-01-02T00:15:00"))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var guest = GuestFactory.Init().Build().Value;
        newEvent.Participate(guest.GuestId);

        //Act
        var result = newEvent.CancelParticipation(guest.GuestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }
}
