using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class SetMaxNumberOfGuestsEventUnitTests
{
    
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    public void SetMaxGuests_Success_WhenEventIsInDraftOrReadyStatus(int maxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft) // Event is in Draft status
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(maxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(maxGuests, newEvent.MaxGuests); // Maximum number of guests is set
    }
    
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    public void SetMaxGuests_Success_WhenEventIsInDraftOrReadyStatusAndValueIsValid(int maxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Ready) // Event is in Draft status
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(maxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(maxGuests, newEvent.MaxGuests); // Maximum number of guests is set
    }

    [Theory]
    [InlineData(10, 15)] 
    [InlineData(20, 25)] 
    [InlineData(30, 30)] 
    public void SetMaxGuests_Success_WhenEventIsActiveAndNewValueIsValidAndGreaterOrEqual(int previousMaxGuests, int newMaxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active) // Event is in Active status
            .WithMaxGuests(previousMaxGuests) // Set previous maximum number of guests
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newMaxGuests, newEvent.MaxGuests); // Maximum number of guests is updated
    }

    [Theory]
    [InlineData(20, 15)] 
    [InlineData(30, 25)] 
    [InlineData(50, 40)] 
    public void SetMaxGuests_Fails_WhenEventIsActiveAndNewValueIsLessThanPrevious(int previousMaxGuests, int newMaxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active) // Event is in Active status
            .WithMaxGuests(previousMaxGuests) // Set previous maximum number of guests
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("The maximum number of guests for an active event cannot be reduced. It may only be increased.", result.Errors.First().Message);
        Assert.Equal(previousMaxGuests, newEvent.MaxGuests); // Maximum number of guests remains unchanged
    }

    [Theory]
    [InlineData(10)] // Attempt to set max guests to 10
    [InlineData(20)] // Attempt to set max guests to 20
    [InlineData(50)] // Attempt to set max guests to 50
    public void SetMaxGuests_Fails_WhenEventIsCancelled(int newMaxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled) // Event is in Cancelled status
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("A cancelled event cannot be modified.", result.Errors.First().Message);
        Assert.NotEqual(newMaxGuests, newEvent.MaxGuests); // Maximum number of guests remains unchanged
    }
    
    /* REQ LOCATION
    [Theory]
    [InlineData(100, 120)] 
    [InlineData(50, 60)]  
    public void SetMaxGuests_Fails_WhenExceedingLocationCapacity(int locationCapacity, int newMaxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithLocationCapacity(locationCapacity) // Set location's max capacity
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("The maximum number of guests cannot exceed the location’s capacity.", result.Errors.First().Message);
        Assert.NotEqual(newMaxGuests, newEvent.MaxGuests);
    }
    */
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void SetMaxGuests_Fails_WhenNumberIsLessThanFive(int newMaxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("The maximum number of guests must be at least 5.", result.Errors.First().Message);
        Assert.NotEqual(newMaxGuests, newEvent.MaxGuests);
    }

    [Theory]
    [InlineData(51)]
    [InlineData(60)]
    [InlineData(100)]
    public void SetMaxGuests_Fails_WhenNumberExceedsFifty(int newMaxGuests)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .Build();

        // Act
        var result = newEvent.SetMaxGuests(newMaxGuests);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("The maximum number of guests cannot exceed 50.", result.Errors.First().Message);
        Assert.NotEqual(newMaxGuests, newEvent.MaxGuests);
    }

    
}