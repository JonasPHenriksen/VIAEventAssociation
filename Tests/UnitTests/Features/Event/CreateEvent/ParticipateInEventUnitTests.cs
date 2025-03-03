using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class ParticipateInEventUnitTests
{
    private DateTime GetTestDate()
    {
        return DateTime.Now.AddYears(1).Date.Add(new TimeSpan(13, 30, 22));
    }

    private DateTime GetPastDateWithOneYearSubtracted()
    {
        return DateTime.Now.AddYears(-1);
    }

    [Fact]
    public void Participate_Success_WhenEventIsActiveAndPublic()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Act
        var result = newEvent.Participate(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.Participants);
    }

    [Fact]
    public void Participate_Fails_WhenEventIsNotActive()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready
        var guestId = GuestId.New();

        // Act
        var result = newEvent.Participate(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenEventIsPrivate()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Act
        var result = newEvent.Participate(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidVisibility", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenEventIsFull()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(1); // Only 1 guest allowed
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active

        // Add one guest to fill the event
        var guestId1 = GuestId.New();
        newEvent.Participate(guestId1);

        var guestId2 = GuestId.New();

        // Act
        var result = newEvent.Participate(guestId2);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenEventHasStarted()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var pastDate = GetPastDateWithOneYearSubtracted();
        newEvent.UpdateTimeRange(pastDate, pastDate.AddHours(4)); // Past event
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Act
        var result = newEvent.Participate(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }

    [Fact]
    public void Participate_Fails_WhenGuestIsAlreadyParticipating()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the participants list
        newEvent.Participate(guestId);

        // Act
        var result = newEvent.Participate(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateParticipation", result.Errors.First().Code);
    }

    // Helper method to set the status using reflection
    private void SetEventStatus(VeaEvent veaEvent, EventStatus status)
    {
        var statusProperty = typeof(VeaEvent).GetProperty("Status", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (statusProperty != null && statusProperty.CanWrite)
        {
            statusProperty.SetValue(veaEvent, status);
        }
        else
        {
            throw new InvalidOperationException("Unable to set Status property.");
        }
    }
}
