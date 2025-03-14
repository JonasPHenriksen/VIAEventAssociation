using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class ActivateEventUnitTests
{
    [Fact]
    public void ActivateEvent_Success_WhenEventIsInDraftStatusAndAllFieldsAreValid()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .WithTitle(new EventTitle("Test"))
            .WithDescription(new EventDescription("Test"))
            .Build();

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, newEvent.Status);
    }
    
    [Fact]
    public void ActivateEvent_Success_WhenEventIsInReadyStatus()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Ready)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

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
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

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
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

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
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

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
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(new EventTitle("test"))
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(4))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

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
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(new EventTitle("test"))
            .WithDescription(new EventDescription("test"))
            .WithMaxGuests(10)
            .WithVisibility(EventVisibility.Public)
            .Build();

        // Act
        var result = newEvent.ActivateEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidTimeRange", result.Errors.First().Code);
    }
    
    
}
