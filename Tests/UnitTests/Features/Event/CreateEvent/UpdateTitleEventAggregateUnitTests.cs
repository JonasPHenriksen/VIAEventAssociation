using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class UpdateTitleEventAggregateUnitTests
{
        [Fact]
    public void UpdateTitle_Success_WhenEventIsInDraftStatus()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;
        var newTitle = "Updated Title";

        // Act
        var result = newEvent.UpdateTitle(newTitle);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, newEvent.Title.Value);
    }

    [Fact]
    public void UpdateTitle_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;
        newEvent.SetStatus(EventStatus.Ready); // Assume a method to set status exists
        var newTitle = "Updated Title";

        // Act
        var result = newEvent.UpdateTitle(newTitle);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, newEvent.Title.Value);
        Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should revert to Draft
    }

    [Fact]
    public void UpdateTitle_Fails_WhenTitleIsEmpty()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;

        // Act
        var result = newEvent.UpdateTitle("");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitle", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTitle_Fails_WhenTitleIsTooShort()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;

        // Act
        var result = newEvent.UpdateTitle("XY");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitleLength", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTitle_Fails_WhenTitleIsTooLong()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;
        var longTitle = new string('A', 76); // 76 characters

        // Act
        var result = newEvent.UpdateTitle(longTitle);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitleLength", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTitle_Fails_WhenEventIsActive()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;
        newEvent.SetStatus(EventStatus.Active); // Assume a method to set status exists

        // Act
        var result = newEvent.UpdateTitle("New Title");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTitle_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = VEAEvent.Create("Initial Title", "Initial Description").Value;
        newEvent.SetStatus(EventStatus.Cancelled); // Assume a method to set status exists

        // Act
        var result = newEvent.UpdateTitle("New Title");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }
}