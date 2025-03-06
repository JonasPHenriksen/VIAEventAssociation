using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class AcceptInvitationUnitTests
{
    private DateTime GetTestDate()
    {
        return DateTime.Now.AddYears(1).Date.Add(new TimeSpan(13, 30, 22));
    }

    [Fact]
    public void AcceptInvitation_Success_WhenEventIsActiveAndGuestIsInvited()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the invited list
        newEvent.InvitedGuests.Add(guestId);

        // Act
        var result = newEvent.AcceptInvitation(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.Participants);
        Assert.DoesNotContain(guestId, newEvent.InvitedGuests);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled
        var guestId = GuestId.New();

        // Add the guest to the invited list
        newEvent.InvitedGuests.Add(guestId);

        // Act
        var result = newEvent.AcceptInvitation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventIsFull()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4));
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId1 = GuestId.New();
        var guestId2 = GuestId.New();
        var guestId3 = GuestId.New();
        var guestId4 = GuestId.New();
        var guestId5 = GuestId.New();
        var guestId6 = GuestId.New();

        newEvent.Participate(guestId1);
        newEvent.Participate(guestId2);
        newEvent.Participate(guestId3);
        newEvent.Participate(guestId4);
        newEvent.Participate(guestId5);
        newEvent.InvitedGuests.Add(guestId6);

        // Act
        var result = newEvent.AcceptInvitation(guestId6);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventIsReady()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready
        var guestId = GuestId.New();

        // Add the guest to the invited list
        newEvent.InvitedGuests.Add(guestId);

        // Act
        var result = newEvent.AcceptInvitation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenEventHasStarted()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var pastDate = DateTime.Now.AddYears(-1);
        newEvent.UpdateTimeRange(pastDate, pastDate.AddHours(4)); // Past event
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the invited list
        newEvent.InvitedGuests.Add(guestId);

        // Act
        var result = newEvent.AcceptInvitation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }

    [Fact]
    public void AcceptInvitation_Fails_WhenGuestIsNotInvited()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New(); // Guest who was never invited

        // Act
        var result = newEvent.AcceptInvitation(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvitationNotFound", result.Errors.First().Code);
    }

    // Helper method to set the status using reflection
    private void SetEventStatus(VeaEvent veaEvent, EventStatus status)
    {
        var statusProperty = typeof(VeaEvent).GetProperty("Status",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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