using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class SetMaxNumberOfGuestsEventUnitTests
{
    [Fact]
    public void SetMaxGuests_Success_WhenEventIsInDraftStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var newMaxGuests = 10;

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, newEvent.MaxGuests);
    }

    [Fact]
    public void SetMaxGuests_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready
        var newMaxGuests = 25;

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, newEvent.MaxGuests);
    }

    [Fact]
    public void SetMaxGuests_Success_WhenEventIsInActiveStatusAndIncreasingGuests()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var newMaxGuests = 30;

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, newEvent.MaxGuests);
    }

    [Fact]
    public void SetMaxGuests_Fails_WhenEventIsActiveAndReducingGuests()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        newEvent.SetMaxGuests(20);
        var newMaxGuests = 15; // Attempt to reduce the number of guests

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidMaxGuests", result.Errors.First().Code);
    }

    [Fact]
    public void SetMaxGuests_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled
        var newMaxGuests = 20;

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void SetMaxGuests_Fails_WhenMaxGuestsIsTooSmall()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var newMaxGuests = 4; // Less than 5

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidMaxGuests", result.Errors.First().Code);
    }

    [Fact]
    public void SetMaxGuests_Fails_WhenMaxGuestsIsTooLarge()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var newMaxGuests = 51; // More than 50

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidMaxGuests", result.Errors.First().Code);
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