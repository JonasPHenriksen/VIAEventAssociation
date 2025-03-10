using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.CreateEvent
{
    public class UpdateTitleEventAggregateUnitTests
    {
        [Fact]
        public void UpdateTitle_Success_WhenEventIsInDraftStatus()
        {
            var newEvent = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
            var newTitle = "Updated Title";

            var result = newEvent.UpdateTitle(newTitle);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, newEvent.Title.Value);
        }

        [Fact]
        public void UpdateTitle_Success_WhenEventIsInReadyStatus()
        {
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Ready)
                .Build();
            var newTitle = "Updated Title";

            var result = newEvent.UpdateTitle(newTitle);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, newEvent.Title.Value);
            Assert.Equal(EventStatus.Draft, newEvent.Status);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenTitleIsEmpty()
        {
            var newEvent = EventFactory.Init().WithStatus(EventStatus.Draft).Build();

            var result = newEvent.UpdateTitle("");

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidTitle", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenTitleIsTooShort()
        {
            var newEvent = EventFactory.Init().WithStatus(EventStatus.Draft).Build();

            var result = newEvent.UpdateTitle("XY");

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidTitleLength", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenTitleIsTooLong()
        {
            var newEvent = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
            var longTitle = new string('A', 76);

            var result = newEvent.UpdateTitle(longTitle);

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidTitleLength", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenEventIsActive()
        {
            var newEvent = EventFactory.Init().WithStatus(EventStatus.Active).Build();

            var result = newEvent.UpdateTitle("New Title");

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenEventIsCancelled()
        {
            var newEvent = EventFactory.Init().WithStatus(EventStatus.Cancelled).Build();

            var result = newEvent.UpdateTitle("New Title");

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }
    }
}
