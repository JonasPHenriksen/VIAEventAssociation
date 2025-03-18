using System;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;
using Xunit;

public class EventValueObjectsTests
{
    //TODO This class contains a lot of duplicate tests
    
    [Fact]
    public void EventId_New_ShouldGenerateNewGuid()
    {
        var eventId1 = EventId.New();
        var eventId2 = EventId.New();
        Assert.NotEqual(eventId1, eventId2);
    }

    [Fact]
    public void EventId_FromString_ShouldReturnEventId_WhenValidGuid()
    {
        var guid = Guid.NewGuid().ToString();
        var eventId = EventId.FromString(guid);
        Assert.Equal(Guid.Parse(guid), eventId.Value);
    }

    [Fact]
    public void EventId_FromString_ShouldThrowException_WhenInvalidGuid()
    {
        Assert.Throws<ArgumentException>(() => EventId.FromString("invalid_guid"));
    }

    [Fact]
    public void EventTitle_Create_ShouldReturnSuccess_WhenValidTitle()
    {
        var result = EventTitle.Create("Valid Title");
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // 76 chars
    public void EventTitle_Create_ShouldReturnFailure_WhenInvalidTitle(string title)
    {
        var result = EventTitle.Create(title);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void EventDescription_Create_ShouldReturnSuccess_WhenValidDescription()
    {
        var result = EventDescription.Create("Valid Description");
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void EventDescription_Create_ShouldReturnFailure_WhenDescriptionTooLong()
    {
        var result = EventDescription.Create(new string('a', 251));
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void EventTimeRange_Create_ShouldReturnSuccess_WhenValidRange()
    {
        var start = DateTime.Today.AddHours(9);
        var end = start.AddHours(2);
        var result = EventTimeRange.Create(start, end, new MockTime.SystemTime()).Value;
        Assert.True(result != null);
    }

    /*
    [Fact]
    public void EventTimeRange_Create_ShouldReturnFailure_WhenStartAfterEnd()
    {
        var start = DateTime.Today.AddHours(10);
        var end = DateTime.Today.AddHours(9);
        var result = EventTimeRange.Create(start, end, new MockTime.SystemTime()).Value;
        Assert.True(result != null);
    }

    [Fact]
    public void EventTimeRange_Create_ShouldReturnFailure_WhenDurationLessThanOneHour()
    {
        var start = DateTime.Today.AddHours(9);
        var end = start.AddMinutes(30);
        var result = EventTimeRange.Create(start, end, new MockTime.SystemTime()).Value;
        Assert.True(result != null);
    }

    [Fact]
    public void EventTimeRange_Create_ShouldReturnFailure_WhenDurationMoreThanTenHours()
    {
        var start = DateTime.Today.AddHours(9);
        var end = start.AddHours(11);
        var result = EventTimeRange.Create(start, end, new MockTime.SystemTime()).Value;
        Assert.True(result != null);
    }

    [Fact]
    public void EventTimeRange_Create_ShouldReturnFailure_WhenStartBefore08AM()
    {
        var start = DateTime.Today.AddHours(7);
        var end = start.AddHours(2);
        var result = EventTimeRange.Create(start, end, new MockTime.SystemTime()).Value;
        Assert.True(result != null);
    }

    [Fact]
    public void EventTimeRange_Create_ShouldReturnFailure_WhenEndAfter01AM()
    {
        var start = DateTime.Today.AddHours(23);
        var end = start.AddHours(3);
        var result = EventTimeRange.Create(start, end, new MockTime.SystemTime()).Value;
        Assert.True(result != null);
    }
    */
}
