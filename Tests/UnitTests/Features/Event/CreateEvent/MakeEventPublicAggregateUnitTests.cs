using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class MakeEventPublicAggregateUnitTests
{
        [Fact]
    public void MakePublic_Success_WhenEventIsInDraftStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;

        // Act
        var result = newEvent.MakePublic();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, newEvent.Visibility);
        Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should remain unchanged
    }

    [Fact]
    public void MakePublic_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Ready); // Set status to Ready

        // Act
        var result = newEvent.MakePublic();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, newEvent.Visibility);
        Assert.Equal(EventStatus.Ready, newEvent.Status); // Status should remain unchanged
    }

    [Fact]
    public void MakePublic_Success_WhenEventIsInActiveStatus()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Active); // Set status to Active

        // Act
        var result = newEvent.MakePublic();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, newEvent.Visibility);
        Assert.Equal(EventStatus.Active, newEvent.Status); // Status should remain unchanged
    }

    [Fact]
    public void MakePublic_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VeaEvent.Create().Value;
        SetEventStatus(newEvent, EventStatus.Cancelled); // Set status to Cancelled

        // Act
        var result = newEvent.MakePublic();

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