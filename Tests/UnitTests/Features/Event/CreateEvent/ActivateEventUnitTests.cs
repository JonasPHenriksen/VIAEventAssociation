using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class ActivateEventUnitTests
{
    private DateTime GetTestDate()
    {
        return DateTime.Now.AddYears(1).Date.Add(new TimeSpan(13, 30, 22));
    }

    [Fact]
    public void ActivateEvent_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, newEvent.Status);
    }

    [Fact]
    public void ActivateEvent_Success_WhenEventIsInDraftStatusAndAllFieldsAreValid()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        newEvent.UpdateTitle("Test");
        newEvent.UpdateDescription("Test");

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, newEvent.Status);
    }

    [Fact]
    public void ActivateEvent_Success_WhenEventIsAlreadyActive()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event will be set to a time in the future
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, newEvent.Status);
    }

    [Fact]
    public void ActivateEvent_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void ActivateEvent_Fails_WhenEventIsInDraftStatusAndTitleIsDefault()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value; // Default title

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitle", result.Errors.First().Code);
    }

    [Fact]
    public void ActivateEvent_Fails_WhenEventIsInDraftStatusAndDescriptionIsNotSet()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value; // Empty description
        newEvent.UpdateTitle("Test");
        var futureDate = GetTestDate();
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4));
        
        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidDescription", result.Errors.First().Code);
    }

    [Fact]
    public void ActivateEvent_Fails_WhenEventIsInDraftStatusAndTimeRangeIsNotSet()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        newEvent.UpdateTitle("Test");
        newEvent.UpdateDescription("Test");

        // Act
        var result = newEvent.ActivateEvent();

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
