using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void CreateEvent_Successfully_CreatesEventWithDefaultValues()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";

        // Act
        var result = VEAEvent.Create(title, description);

        // Assert
        Assert.True(result.IsSuccess);
        var newEvent = result.Value;
        Assert.NotNull(newEvent);
        Assert.Equal(EventStatus.Draft, newEvent.Status);
        Assert.Equal(EventVisibility.Private, newEvent.Visibility);
        Assert.Equal(5, newEvent.MaxGuests);
        Assert.Equal(title, newEvent.Title.Value);
        Assert.Equal(description, newEvent.Description.Value);
    }

    [Fact]
    public void CreateEvent_Fails_WhenTitleIsEmpty()
    {
        // Arrange
        var title = "";
        var description = "This is a test event.";

        // Act
        var result = VEAEvent.Create(title, description);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitle", result.Errors.First().Code);
    }

    [Fact]
    public void CreateEvent_Fails_WhenDescriptionIsEmpty()
    {
        // Arrange
        var title = "Test Event";
        var description = "";

        // Act
        var result = VEAEvent.Create(title, description);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidDescription", result.Errors.First().Code);
    }

    [Fact]
    public void PublishEvent_Successfully_ChangesStatusToPublished()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";
        var newEvent = VEAEvent.Create(title, description).Value;

        // Act
        var result = newEvent.Publish();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Published, newEvent.Status);
    }

    [Fact]
    public void PublishEvent_Fails_WhenEventIsNotInDraftStatus()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";
        var newEvent = VEAEvent.Create(title, description).Value;
        newEvent.Publish(); // Change status to Published

        // Act
        var result = newEvent.Publish();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidStatus", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateTitle_Successfully_ChangesTitle()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";
        var newEvent = VEAEvent.Create(title, description).Value;
        var newTitle = "Updated Title";

        // Act
        var result = newEvent.UpdateTitle(newTitle);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, newEvent.Title.Value);
    }

    [Fact]
    public void UpdateTitle_Fails_WhenTitleIsEmpty()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";
        var newEvent = VEAEvent.Create(title, description).Value;
        var newTitle = "";

        // Act
        var result = newEvent.UpdateTitle(newTitle);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTitle", result.Errors.First().Code);
    }

    [Fact]
    public void UpdateDescription_Successfully_ChangesDescription()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";
        var newEvent = VEAEvent.Create(title, description).Value;
        var newDescription = "Updated Description";

        // Act
        var result = newEvent.UpdateDescription(newDescription);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, newEvent.Description.Value);
    }

    [Fact]
    public void UpdateDescription_Fails_WhenDescriptionIsEmpty()
    {
        // Arrange
        var title = "Test Event";
        var description = "This is a test event.";
        var newEvent = VEAEvent.Create(title, description).Value;
        var newDescription = "";

        // Act
        var result = newEvent.UpdateDescription(newDescription);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidDescription", result.Errors.First().Code);
    }
}