using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class InviteGuestUnitTests
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
    public void InviteGuest_Success_WhenEventIsReady()
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
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.InvitedGuests);
    }

    [Fact]
    public void InviteGuest_Success_WhenEventIsActive()
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
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(guestId, newEvent.InvitedGuests);
    }

    [Fact]
    public void InviteGuest_Fails_WhenEventIsDraft()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Draft); // Set status to Draft
        var guestId = GuestId.New();

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
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled
        var guestId = GuestId.New();

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
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(5); // Only 1 guest allowed
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active

        // Add one guest to fill the event
        var guestId1 = GuestId.New();
        var guestId2 = GuestId.New();
        var guestId3 = GuestId.New();
        var guestId4 = GuestId.New();
        var guestId5 = GuestId.New();
        var guestId6 = GuestId.New();
        newEvent.InviteGuest(guestId1);
        newEvent.InviteGuest(guestId2);
        newEvent.InviteGuest(guestId3);
        newEvent.InviteGuest(guestId4);
        newEvent.InviteGuest(guestId5);

        // Act
        var result = newEvent.InviteGuest(guestId6);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NoMoreRoom", result.Errors.First().Code);
    }

    [Fact]
    public void InviteGuest_Fails_WhenGuestIsAlreadyInvited()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var guestId = GuestId.New();

        // Add the guest to the invited guests list
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
        var result = newEvent.InviteGuest(guestId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AlreadyParticipating", result.Errors.First().Code);
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
