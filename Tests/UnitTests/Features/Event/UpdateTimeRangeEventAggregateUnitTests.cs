using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.UpdateEventTimeRange
{
    public class UpdateEventTimeRangeUnitTests
    {
        private DateTime GetFutureDate(string time)
        {
            return DateTime.Now.AddYears(1).Date + TimeSpan.Parse(time);
        }

        [Theory]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        [InlineData("10:00", "20:00")]
        [InlineData("13:00", "23:00")]
        [InlineData("18:00", "01:00")]
        public void UpdateTimeRange_Success_WhenEventIsInDraftStatusAndConditionsAreMet(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(startTime, endTime);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime > startTime ? endTime : endTime.AddDays(1), newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("19:00", "01:00")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        public void UpdateTimeRange_Success_WhenEventIsInDraftStatusAndCrossesMidnightAndConditionsAreMet(string startTimeStr, string endTimeStr)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            // Act
            var result = newEvent.UpdateTimeRange(startTime, endTime);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        [InlineData("10:00", "20:00")]
        [InlineData("13:00", "23:00")]
        [InlineData("19:00", "01:00")]
        public void UpdateTimeRange_Success_WhenEventIsReadyAndChangesToDraft(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Ready)
                .Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
            Assert.Equal(EventStatus.Draft, newEvent.Status);
        }

        [Theory]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        [InlineData("10:00", "20:00")]
        [InlineData("13:00", "23:00")]
        [InlineData("19:00", "01:00")]
        public void UpdateTimeRange_Success_WhenStartTimeIsInTheFuture(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        [InlineData("10:00", "20:00")]
        [InlineData("13:00", "23:00")]
        [InlineData("19:00", "01:00")]
        public void UpdateTimeRange_Success_WhenDurationIs10HoursOrLess(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);
            var duration = endTime - startTime;

            Assert.True(duration.TotalHours <= 10);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.True(result.IsSuccess);
            Assert.Equal(startTime, newEvent.TimeRange.Start);
            Assert.Equal(endTime, newEvent.TimeRange.End);
        }

        [Theory]
        [InlineData("19:00", "01:00")]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        public void UpdateTimeRange_Fails_WhenStartDateIsAfterEndDate(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr).AddDays(1);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Start date must be before end date.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("19:00", "14:00")]
        [InlineData("16:00", "00:00")]
        [InlineData("19:00", "18:59")]
        [InlineData("12:00", "10:10")]
        [InlineData("08:00", "00:30")]
        public void UpdateTimeRange_Fails_WhenStartTimeIsAfterEndTime(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Start time must be before end time.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("14:00", "14:50")]
        [InlineData("18:00", "18:59")]
        [InlineData("12:00", "12:30")]
        [InlineData("08:00", "08:00")]
        public void UpdateTimeRange_Fails_WhenDurationIsLessThanOneHour(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);
            var duration = endTime - startTime;

            Assert.True(duration.TotalMinutes < 60);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Event duration must be at least 1 hour.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("23:30", "00:15")]
        [InlineData("23:01", "00:00")]
        [InlineData("23:59", "00:01")]
        public void UpdateTimeRange_Fails_WhenDurationIsLessThanOneHourAndStartDateIsBeforeEndDate(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr).AddDays(1);
            var duration = endTime - startTime;

            Assert.True(duration.TotalMinutes < 60);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Event duration must be at least 1 hour.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("07:50", "14:00")]
        [InlineData("07:59", "15:00")]
        [InlineData("01:01", "08:30")]
        [InlineData("05:59", "07:59")]
        [InlineData("00:59", "07:59")]
        public void UpdateTimeRange_Fails_WhenStartTimeIsBeforeEightAM(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot occur between 01:01 AM and 07:59 AM.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("23:50", "01:01")]
        [InlineData("22:00", "07:59")]
        [InlineData("23:00", "02:30")]
        public void UpdateTimeRange_Fails_WhenEndTimeIsAfterOneAM(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime.AddDays(1));

            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot occur between 01:01 AM and 07:59 AM.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        [InlineData("10:00", "20:00")]
        [InlineData("13:00", "23:00")]
        [InlineData("19:00", "01:00")]
        public void UpdateTimeRange_Fails_WhenEventIsActive(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Active)
                .Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Time range can only be updated when the event is in Draft or Ready status.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("19:00", "23:59")]
        [InlineData("12:00", "16:30")]
        [InlineData("08:00", "12:15")]
        [InlineData("10:00", "20:00")]
        [InlineData("13:00", "23:00")]
        [InlineData("19:00", "01:00")]
        public void UpdateTimeRange_Fails_WhenEventIsCancelled(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Cancelled)
                .Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Time range can only be updated when the event is in Draft or Ready status.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("08:00", "18:01")]
        [InlineData("14:59", "01:00")]
        [InlineData("14:00", "00:01")]
        [InlineData("10:00", "21:00")]
        public void UpdateTimeRange_Fails_WhenEventDurationIsLongerThanTenHours(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);
            
            if (startTime > endTime)
            {
                endTime = endTime.AddDays(1);
            }

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Event duration cannot exceed 10 hours.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("2023/08/25 07:50", "2023/08/25 14:00")]
        [InlineData("2023/08/25 07:59", "2023/08/25 15:00")]
        [InlineData("2023/08/25 01:01", "2023/08/25 08:30")]
        [InlineData("2023/08/25 05:59", "2023/08/25 07:59")]
        public void UpdateTimeRange_Fails_WhenStartTimeIsInThePast(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = DateTime.Parse(startTimeStr);
            var endTime = DateTime.Parse(endTimeStr);

            Assert.True(startTime < DateTime.Now);

            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Events cannot be started in the past.", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("00:30", "08:30")]
        [InlineData("23:59", "08:01")]
        [InlineData("01:00", "08:00")]
        public void UpdateTimeRange_Fails_WhenEventDurationSpansInvalidTime(string startTimeStr, string endTimeStr)
        {
            var newEvent = EventFactory.Init().Build();
            var startTime = GetFutureDate(startTimeStr);
            var endTime = GetFutureDate(endTimeStr);
            
            if (startTime > endTime)
            {
                endTime = endTime.AddDays(1);
            }
            
            var result = newEvent.UpdateTimeRange(startTime, endTime);

            Assert.False(result.IsSuccess);
            Assert.Equal("Event cannot span from 1:00 AM to 08:00 AM.", result.Errors.First().Message);
        }
    }
}