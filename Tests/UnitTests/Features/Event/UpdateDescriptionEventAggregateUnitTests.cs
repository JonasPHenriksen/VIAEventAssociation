using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class UpdateDescriptionEventAggregateUnitTests
{
    [Fact]
    public void UpdateDescription_Success_WhenEventIsInDraftStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var newDescription = "Updated Description";

        // Act
        var result = newEvent.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, newEvent.Description.Value);
    }

    [Fact]
    public void UpdateDescription_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready
        var newDescription = "Updated Description";

        // Act
        var result = newEvent.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, newEvent.Description.Value);
        Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should revert to Draft
    }

    [Fact]
    public void UpdateDescription_Success_WhenDescriptionIsEmpty()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;

        // Act
        var result = newEvent.UpdateDescription("");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("", newEvent.Description.Value);
    }

    [Fact]
    public void UpdateDescription_Fails_WhenDescriptionIsTooLong()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        var longDescription = new string('A', 251); // 251 characters

        // Act
        var result = newEvent.UpdateDescription(longDescription);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidDescriptionLength", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateDescription_Fails_WhenEventIsActive()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active

        // Act
        var result = newEvent.UpdateDescription("New Description");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateDescription_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled

        // Act
        var result = newEvent.UpdateDescription("New Description");

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