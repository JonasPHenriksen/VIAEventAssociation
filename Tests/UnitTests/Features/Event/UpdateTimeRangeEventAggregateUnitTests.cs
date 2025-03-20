using UnitTests.Factories;
using UnitTests.Fakes;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.UpdateEventTimeRange
{
    public class UpdateEventTimeRangeUnitTests
    {
        [Theory]
        [InlineData("2023-08-25T19:00:00", "2023-08-25T23:59:00")]
        [InlineData("2023-08-25T12:00:00", "2023-08-25T16:30:00")]
        [InlineData("2023-08-25T08:00:00", "2023-08-25T12:15:00")]
        [InlineData("2023-08-25T10:00:00", "2023-08-25T20:00:00")]
        [InlineData("2023-08-25T13:00:00", "2023-08-25T23:00:00")]
        [InlineData("2023-08-25T18:00:00", "2023-08-26T01:00:00")]
        public void UpdateTimeRange_Success_WhenEventIsInDraftStatusAndConditionsAreMet(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(EventTimeRange.Create(startTime, endTime, new MockTime.SystemTime()).Value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime > startTime ? endTime : endTime.AddDays(1), newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("2023-08-25T19:00:00", "2023-08-26T01:00:00")]
        [InlineData("2023-08-25T12:00:00", "2023-08-25T16:30:00")]
        [InlineData("2023-08-25T08:00:00", "2023-08-25T12:15:00")]
        public void UpdateTimeRange_Success_WhenEventIsInDraftStatusAndCrossesMidnightAndConditionsAreMet(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(EventTimeRange.Create(startTime, endTime, new MockTime.SystemTime()).Value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("2023-08-25T19:00:00", "2023-08-25T23:59:00")]
        [InlineData("2023-08-25T12:00:00", "2023-08-25T16:30:00")]
        [InlineData("2023-08-25T08:00:00", "2023-08-25T12:15:00")]
        [InlineData("2023-08-25T10:00:00", "2023-08-25T20:00:00")]
        [InlineData("2023-08-25T13:00:00", "2023-08-25T23:00:00")]
        [InlineData("2023-08-25T19:00:00", "2023-08-26T01:00:00")]
        public void UpdateTimeRange_Success_WhenEventIsReadyAndChangesToDraft(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Ready)
                .Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(EventTimeRange.Create(startTime, endTime, new MockTime.SystemTime()).Value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
            Assert.Equal(EventStatus.Draft, newEvent.Status);
        }


        [Theory]
        [InlineData("2023-08-25T19:00:00", "2023-08-25T23:59:00")]
        [InlineData("2023-08-25T12:00:00", "2023-08-25T16:30:00")]
        [InlineData("2023-08-25T08:00:00", "2023-08-25T12:15:00")]
        [InlineData("2023-08-25T10:00:00", "2023-08-25T20:00:00")]
        [InlineData("2023-08-25T13:00:00", "2023-08-25T23:00:00")]
        [InlineData("2023-08-25T19:00:00", "2023-08-26T01:00:00")]
        public void UpdateTimeRange_Success_WhenStartTimeIsInTheFuture(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var newEvent = EventFactory.Init().Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(EventTimeRange.Create(startTime, endTime, new MockTime.SystemTime()).Value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("2022-01-01T12:00:00", "2022-01-01T16:30:00")] // Same day
        [InlineData("2022-01-01T08:00:00", "2022-01-01T12:15:00")] // Same day
        [InlineData("2022-01-01T10:00:00", "2022-01-01T20:00:00")] // Same day
        [InlineData("2022-01-01T13:00:00", "2022-01-01T23:00:00")] // Same day
        [InlineData("2022-01-01T19:00:00", "2022-01-02T01:00:00")] // Spans midnight
        public void UpdateTimeRange_Success_WhenDurationIs10HoursOrLess(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime(); // Use the mock system time
            var newEvent = EventFactory.Init().Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);
            var duration = endTime - startTime;
            
            // Assert preconditions
            Assert.True(duration.TotalHours <= 10);
            
            // Act
            var resultTimeRange = EventTimeRange.Create(startTime, endTime, mockSystemTime);
            var result = newEvent.UpdateTimeRange(resultTimeRange.Value);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("2020-01-02T19:00:00", "2020-01-01T01:00:00")]
        [InlineData("2020-01-02T19:00:00", "2020-01-01T23:59:00")]
        [InlineData("2020-01-02T12:00:00", "2020-01-01T16:30:00")]
        [InlineData("2020-01-02T08:00:00", "2020-01-01T12:15:00")]
        public void UpdateTimeRange_Fails_WhenStartDateIsAfterEndDate(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Events cannot be started in the past.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("2020-01-01T19:00:00", "2020-01-01T14:00:00")]
        [InlineData("2020-01-01T16:00:00", "2020-01-01T00:00:00")]
        [InlineData("2020-01-01T19:00:00", "2020-01-01T18:59:00")]
        [InlineData("2020-01-01T12:00:00", "2020-01-01T10:10:00")]
        [InlineData("2020-01-01T08:00:00", "2020-01-01T00:30:00")]
        public void UpdateTimeRange_Fails_WhenStartTimeIsAfterEndTime(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Events cannot be started in the past.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("2022-01-01T14:00:00", "2022-01-01T14:50:00")]
        [InlineData("2022-01-01T18:00:00", "2022-01-01T18:59:00")]
        [InlineData("2022-01-01T12:00:00", "2022-01-01T12:30:00")]
        [InlineData("2022-01-01T08:00:00", "2022-01-01T08:00:00")]
        public void UpdateTimeRange_Fails_WhenDurationIsLessThanOneHour(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);
            var duration = endTime - startTime;

            Assert.True(duration.TotalMinutes < 60);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event duration must be at least 1 hour.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("2022-01-01T23:30:00", "2022-01-02T00:15:00")]
        [InlineData("2022-01-01T23:01:00", "2022-01-02T00:00:00")]
        [InlineData("2022-01-01T23:59:00", "2022-01-02T00:01:00")]
        public void UpdateTimeRange_Fails_WhenDurationIsLessThanOneHourAndStartDateIsBeforeEndDate(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);
            var duration = endTime - startTime;

            Assert.True(duration.TotalMinutes < 60);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event duration must be at least 1 hour.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("2022-01-01T07:50:00", "2022-01-01T14:00:00")]
        [InlineData("2022-01-01T07:59:00", "2022-01-01T15:00:00")]
        [InlineData("2022-01-01T01:01:00", "2022-01-01T08:30:00")]
        [InlineData("2022-01-01T05:59:00", "2022-01-01T07:59:00")]
        [InlineData("2022-01-01T00:59:00", "2022-01-01T07:59:00")]
        public void UpdateTimeRange_Fails_WhenStartTimeIsBeforeEightAM(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);
            
            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot occur between 01:01 AM and 07:59 AM.", result.Errors.First().Message);
        }


        [Theory]
        [InlineData("2022-01-01T23:50:00", "2022-01-02T01:01:00")]
        [InlineData("2022-01-01T22:00:00", "2022-01-02T07:59:00")]
        [InlineData("2022-01-01T23:00:00", "2022-01-02T02:30:00")]
        public void UpdateTimeRange_Fails_WhenEndTimeIsAfterOneAM(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);
            
            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot occur between 01:01 AM and 07:59 AM.", result.Errors.First().Message);
        }


        [Theory]
        [InlineData("2020-01-01T19:00:00", "2020-01-01T23:59:00")]
        [InlineData("2020-01-01T12:00:00", "2020-01-01T16:30:00")]
        [InlineData("2020-01-01T08:00:00", "2020-01-01T12:15:00")]
        [InlineData("2020-01-01T10:00:00", "2020-01-01T20:00:00")]
        [InlineData("2020-01-01T13:00:00", "2020-01-01T23:00:00")]
        [InlineData("2020-01-01T19:00:00", "2020-01-02T01:00:00")]
        public void UpdateTimeRange_Fails_WhenEventIsActive(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Active)
                .Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(EventTimeRange.Create(startTime, endTime, mockSystemTime).Value);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Time range can only be updated when the event is in Draft or Ready status.", result.Errors.First().Message);
        }


        [Theory]
        [InlineData("2020-01-01T19:00:00", "2020-01-01T23:59:00")]
        [InlineData("2020-01-01T12:00:00", "2020-01-01T16:30:00")]
        [InlineData("2020-01-01T08:00:00", "2020-01-01T12:15:00")]
        [InlineData("2020-01-01T10:00:00", "2020-01-01T20:00:00")]
        [InlineData("2020-01-01T13:00:00", "2020-01-01T23:00:00")]
        [InlineData("2020-01-01T19:00:00", "2020-01-02T01:00:00")]
        public void UpdateTimeRange_Fails_WhenEventIsCancelled(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Cancelled)
                .Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(EventTimeRange.Create(startTime, endTime, mockSystemTime).Value);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Time range can only be updated when the event is in Draft or Ready status.", result.Errors.First().Message);
        }


        [Theory]
        [InlineData("2022-01-01T08:00:00", "2022-01-01T18:01:00")]
        [InlineData("2022-01-01T14:59:00", "2022-01-02T01:00:00")]
        [InlineData("2022-01-01T14:00:00", "2022-01-02T00:01:00")]
        [InlineData("2022-01-01T10:00:00", "2022-01-01T21:00:00")]
        public void UpdateTimeRange_Fails_WhenEventDurationIsLongerThanTenHours(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event duration cannot exceed 10 hours.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("2023-08-25T07:50:00", "2023-08-25T14:00:00")]
        [InlineData("2023-08-25T07:59:00", "2023-08-25T15:00:00")]
        [InlineData("2023-08-25T01:01:00", "2023-08-25T08:30:00")]
        [InlineData("2023-08-25T05:59:00", "2023-08-25T07:59:00")]
        public void UpdateTimeRange_Fails_WhenStartTimeIsInThePast(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot occur between 01:01 AM and 07:59 AM.", result.Errors.First().Message);
        }


        [Theory]
        [InlineData("2023-08-25T00:30:00", "2023-08-25T08:30:00")]
        [InlineData("2023-08-25T23:59:00", "2023-08-26T08:01:00")]
        [InlineData("2023-08-25T01:00:00", "2023-08-25T08:00:00")]
        public void UpdateTimeRange_Fails_WhenEventDurationSpansInvalidTime(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var mockSystemTime = new MockTime.SystemTime();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            // Act
            var result = EventTimeRange.Create(startTime, endTime, mockSystemTime);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot span from 1:00 AM to 08:00 AM.", result.Errors.First().Message);
        }

    }
}