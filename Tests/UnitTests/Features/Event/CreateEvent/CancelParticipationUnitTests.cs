using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class CancelParticipationUnitTests
{
    private DateTime GetTestDate()
    {
        return DateTime.Now.AddYears(1).Date.Add(new TimeSpan(13, 30, 22));
    }

    [Fact]
    public void CancelParticipation_Success_WhenGuestIsParticipating()
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
        var result = newEvent.CancelParticipation(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(guestId, newEvent.Participants);
    }

    [Fact]
    public void CancelParticipation_Success_WhenGuestIsNotParticipating()
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
        var result = newEvent.CancelParticipation(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(guestId, newEvent.Participants);
    }
    
    [Fact]
    public void CancelParticipation_Fails_WhenEventHasStarted()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        newEvent.UpdateTimeRange(DateTime.Now.AddSeconds(3), DateTime.Now.AddSeconds(8));
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the participants list
        newEvent.Participate(guestId);

        //TODO MAKE A PAST EVENT HACK SINCE WE DON'T WANT TO WAIT AN HOUR

        //Act
        var result = newEvent.CancelParticipation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
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
