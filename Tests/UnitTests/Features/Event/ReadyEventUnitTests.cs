using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.CreateEvent;

public class ReadyEventUnitTests
{

    [Fact]
    public void ReadyEvent_Success_WhenAllRequiredDataIsValid()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(new EventTitle("Event Title"))
            .WithDescription(new EventDescription("Event Description"))
            .WithTimeRange(DateTime.Parse("2025-06-01T12:00:00Z"), DateTime.Parse("2025-06-01T15:00:00Z"))
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .Build();

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Ready, newEvent.Status);
    }

    [Theory]
    [InlineData("", "Description", "2025-06-01T12:00:00Z", "2025-06-01T15:00:00Z", EventVisibility.Private, 10, "The title cannot be empty")]
    [InlineData("Working Title", "Description", "2025-06-01T12:00:00Z", "2025-06-01T15:00:00Z", EventVisibility.Private, 10, "The title must be changed from the default value.")]
    [InlineData("Title", "", "2025-06-01T12:00:00Z", "2025-06-01T15:00:00Z", EventVisibility.Private, 10, "The description must be set.")]
    [InlineData("Title", "Description", "0001-01-01T00:00:00Z", "2025-06-01T15:00:00Z", EventVisibility.Private, 10, "An event in the past cannot be readied.")]
    [InlineData("Title", "Description", "2025-06-01T12:00:00Z", "0001-01-01T00:00:00Z", EventVisibility.Private, 10, "An event in the past cannot be readied.")]
    //[InlineData("Title", "Description", "2025-06-01T12:00:00Z", "2025-06-01T15:00:00Z", null, 10, "Visibility must be set.")] visibility cannot be null
    [InlineData("Title", "Description", "2025-06-01T12:00:00Z", "2025-06-01T15:00:00Z", EventVisibility.Public, 4, "The maximum number of guests must be between 5 and 50.")]
    [InlineData("Title", "Description", "2025-06-01T12:00:00Z", "2025-06-01T15:00:00Z", EventVisibility.Public, 51, "The maximum number of guests must be between 5 and 50.")]
    public void ReadyEvent_Fails_WhenRequiredDataIsMissingOrInvalid(
        string title, string description, string startTime, string endTime, EventVisibility visibility, int maxGuests, string expectedError)
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(new EventTitle(title))
            .WithDescription(new EventDescription(description))
            .WithTimeRange(DateTime.Parse(startTime), DateTime.Parse(endTime))
            .WithVisibility(visibility)
            .WithMaxGuests(maxGuests)
            .Build();

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Errors.First().Message);
        Assert.Equal(EventStatus.Draft, newEvent.Status);
    }
    
    [Fact]
    public void ReadyEvent_Fails_WhenEventIsCancelled()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .WithTitle(new EventTitle("Event Title"))
            .WithDescription(new EventDescription("Event Description"))
            .WithTimeRange(DateTime.UtcNow.AddYears(1), DateTime.UtcNow.AddYears(1).AddHours(3))
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .Build();

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("A cancelled event cannot be readied.", result.Errors.First().Message);
        Assert.Equal(EventStatus.Cancelled, newEvent.Status);
    }
    
    [Fact]
    public void ReadyEvent_Fails_WhenEventIsInThePast()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(new EventTitle("Event Title"))
            .WithDescription(new EventDescription("Event Description"))
            .WithTimeRange(DateTime.Parse("2020-01-01T23:30:00"), DateTime.Parse("2020-01-02T00:15:00")) // Start time in the past (Check Mock Time)
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .Build();

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An event in the past cannot be readied.", result.Errors.First().Message);
        Assert.Equal(EventStatus.Draft, newEvent.Status);
    }
    
    [Fact]
    public void ReadyEvent_Fails_WhenTitleIsDefault()
    {
        // Arrange
        var newEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(new EventTitle("Working Title")) // Default title
            .WithDescription(new EventDescription("Event Description"))
            .WithTimeRange(DateTime.Now.AddYears(1), DateTime.Now.AddYears(1).AddHours(3))
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .Build();

        // Act
        var result = newEvent.ReadyEvent();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("The title must be changed from the default value.", result.Errors.First().Message);
        Assert.Equal(EventStatus.Draft, newEvent.Status);
    }


}
