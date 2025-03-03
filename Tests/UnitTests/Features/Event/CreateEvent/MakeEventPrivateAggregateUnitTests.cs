using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class MakeEventPrivateAggregateUnitTests
{
        [Fact]
    public void MakePrivate_Success_WhenEventIsInDraftStatusAndAlreadyPrivate()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;

        // Act
        var result = newEvent.MakePrivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Private, newEvent.Visibility);
        Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should remain unchanged
    }

    [Fact]
    public void MakePrivate_Success_WhenEventIsInReadyStatusAndPublic()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventVisibility(newEvent, EventVisibility.Public); // Set visibility to Public
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready

        // Act
        var result = newEvent.MakePrivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Private, newEvent.Visibility);
        Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should revert to Draft
    }

    [Fact]
    public void MakePrivate_Fails_WhenEventIsActive()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active

        // Act
        var result = newEvent.MakePrivate();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void MakePrivate_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled

        // Act
        var result = newEvent.MakePrivate();

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

    // Helper method to set the visibility using reflection
    private void SetEventVisibility(VeaEvent veaEvent, EventVisibility visibility)
    {
        var visibilityProperty = typeof(VeaEvent).GetProperty("Visibility", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (visibilityProperty != null && visibilityProperty.CanWrite)
        {
            visibilityProperty.SetValue(veaEvent, visibility);
        }
        else
        {
            throw new InvalidOperationException("Unable to set Visibility property.");
        }
    }
}