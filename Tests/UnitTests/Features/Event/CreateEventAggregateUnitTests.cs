using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateEventAggregateUnitTests
{
    [Fact]
    public void CreateEvent_Success_SetStatusToDraftAndMaxNumberOfGuestsTo5()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        Assert.True(result.IsSuccess);
        var newEvent = result.Value;
        Assert.NotNull(newEvent);
        Assert.Equal(EventStatus.Draft, newEvent.Status);
        Assert.Equal(5, newEvent.MaxGuests);
        Assert.Equal(EventVisibility.Private, newEvent.Visibility);
        Assert.Equal("Working Title", newEvent.Title.Value);
        Assert.Equal("", newEvent.Description.Get);
    }
    
    [Fact]
    public void CreateEvent_Success_SetEventTitleToWorkingTitle()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        Assert.True(result.IsSuccess);
        var newEvent = result.Value;
        Assert.NotNull(newEvent);
        Assert.Equal("Working Title", newEvent.Title.Value);
    }
    
    [Fact]
    public void CreateEvent_Success_SetEventDescriptionToEmptyString()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        Assert.True(result.IsSuccess);
        var newEvent = result.Value;
        Assert.NotNull(newEvent);
        Assert.Equal("", newEvent.Description.Get);
    }
    
    [Fact]
    public void CreateEvent_Success_SetEventVisibilityToPrivate()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        Assert.True(result.IsSuccess);
        var newEvent = result.Value;
        Assert.NotNull(newEvent);
        Assert.Equal(EventVisibility.Private, newEvent.Visibility);
    }
}