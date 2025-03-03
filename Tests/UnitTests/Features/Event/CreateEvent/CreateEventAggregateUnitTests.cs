using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void CreateEvent_Successfully_CreatesEventWithDefaultValues()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        Assert.True(result.IsSuccess);
        var newEvent = result.Value;
        Assert.NotNull(newEvent);
        Assert.Equal(EventStatus.Draft, newEvent.Status);
        Assert.Equal(EventVisibility.Private, newEvent.Visibility);
        Assert.Equal(5, newEvent.MaxGuests);
        Assert.Equal("Working Title", newEvent.Title.Value);
        Assert.Equal("", newEvent.Description.Value);
    }
    
    [Fact]
    public void PublishEvent_Successfully_ChangesStatusToPublished()
    {

        var newEvent = VeaEvent.Create().Value;

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
        var newEvent = VeaEvent.Create().Value;
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

        var description = "This is a test event.";
        var newEvent = VeaEvent.Create().Value;
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
        
        var newEvent = VeaEvent.Create().Value;
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

        var newEvent = VeaEvent.Create().Value;
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
        var newEvent = VeaEvent.Create().Value;
        var newDescription = "";

        // Act
        var result = newEvent.UpdateDescription(newDescription);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidDescription", result.Errors.First().Code);
    }
}