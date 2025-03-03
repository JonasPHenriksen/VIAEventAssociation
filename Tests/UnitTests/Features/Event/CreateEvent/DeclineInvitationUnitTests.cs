using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class DeclineInvitationUnitTests
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
    public void DeclineInvitation_Success_WhenGuestHasPendingInvitation()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the invited list
        newEvent.InvitedGuests.Add(guestId);

        // Act
        var result = newEvent.DeclineInvitation(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.DeclinedGuests);
        Assert.DoesNotContain(guestId, newEvent.InvitedGuests);
    }

    [Fact]
    public void DeclineInvitation_Success_WhenGuestHasAcceptedInvitation()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the accepted participants list
        newEvent.Participate(guestId);

        // Act
        var result = newEvent.DeclineInvitation(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.DeclinedGuests);
        Assert.DoesNotContain(guestId, newEvent.Participants);
    }

    [Fact]
    public void DeclineInvitation_Fails_WhenInvitationNotFound()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New(); // Guest who is not invited

        // Act
        var result = newEvent.DeclineInvitation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationNotFound", result.Errors.First().Code);
    }

    [Fact]
    public void DeclineInvitation_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled
        var guestId = GuestId.New();

        // Add the guest to the invited list
        newEvent.InvitedGuests.Add(guestId);

        // Act
        var result = newEvent.DeclineInvitation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("CannotDeclineInvitation", result.Errors.First().Code);
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
