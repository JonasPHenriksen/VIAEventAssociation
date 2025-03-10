using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.CreateEvent;

public class ReadyEventUnitTests
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
    public void ReadyEvent_Success_WhenAllFieldsAreValid()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var futureDate = GetTestDate();
        newEvent.UpdateTitle("Test Title");
        newEvent.UpdateDescription("Test Description");
        newEvent.UpdateTimeRange(futureDate, futureDate.AddHours(4)); // Event set to a future date
        newEvent.SetMaxGuests(10);
        newEvent.MakePublic();

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Ready, newEvent.Status);
    }

    [Fact]
    public void ReadyEvent_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void ReadyEvent_Fails_WhenEventIsNotInDraftStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void ReadyEvent_Fails_WhenTitleIsDefault()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value; // Default title

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitle", result.Errors.First().Code);
    }

    [Fact]
    public void ReadyEvent_Fails_WhenTimeRangeIsNotSet()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        newEvent.UpdateTitle("Test Title");
        newEvent.UpdateDescription("Test Description");

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }

    [Fact]
    public void ReadyEvent_Fails_WhenEventIsInThePast()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var pastDate = GetPastDateWithOneYearSubtracted();
        newEvent.UpdateTimeRange(pastDate, pastDate.AddHours(4)); // Past event
        newEvent.UpdateTitle("Test Title");
        newEvent.UpdateDescription("Test Description");

        // Act
        var result = newEvent.ReadyEvent();

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
