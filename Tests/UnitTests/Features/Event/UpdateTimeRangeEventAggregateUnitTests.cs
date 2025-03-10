using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.CreateEvent;

public class UpdateTimeRangeEventAggregateUnitTests
{
    private DateTime GetFutureDateWithOneDayAdded()
    {
        return DateTime.Now.AddDays(1);
    }

    private DateTime GetPastDateWithOneDaySubtracted()
    {
        return DateTime.Now.AddDays(-1);
    }

    [Fact]
    public void UpdateTimeRange_Success_WhenEventIsInDraftStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var start = GetFutureDateWithOneDayAdded();
        var end = start.AddHours(4);

        // Act
        var result = newEvent.UpdateTimeRange(start, end);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(start, newEvent.TimeRange?.Start);
        Assert.Equal(end, newEvent.TimeRange?.End);
    }

    [Fact]
    public void UpdateTimeRange_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready
        var start = GetFutureDateWithOneDayAdded();
        var end = start.AddHours(4);

        // Act
        var result = newEvent.UpdateTimeRange(start, end);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(start, newEvent.TimeRange?.Start);
        Assert.Equal(end, newEvent.TimeRange?.End);
        Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should revert to Draft
    }

    [Fact]
    public void UpdateTimeRange_Fails_WhenStartTimeIsAfterEndTime()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var start = GetFutureDateWithOneDayAdded();
        var end = GetPastDateWithOneDaySubtracted();

        // Act
        var result = newEvent.UpdateTimeRange(start, end);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTimeRange_Fails_WhenDurationIsTooShort()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var start = GetFutureDateWithOneDayAdded();
        var end = start.AddMinutes(50); // Duration is too short

        // Act
        var result = newEvent.UpdateTimeRange(start, end);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidDuration", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTimeRange_Fails_WhenStartTimeIsTooEarly()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var start = new DateTime(2023, 8, 25, 7, 59, 0); // Too early for start time
        var end = start.AddHours(8);

        // Act
        var result = newEvent.UpdateTimeRange(start, end);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStartTime", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTimeRange_Fails_WhenEventIsActive()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active
        var start = GetFutureDateWithOneDayAdded();
        var end = start.AddHours(4);

        // Act
        var result = newEvent.UpdateTimeRange(start, end);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
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
