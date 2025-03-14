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
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var newTitle = "Updated Title";

            // Act
            var result = newEvent.UpdateTitle(newTitle);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, newEvent.Title.Value);
        }

        [Fact]
        public void UpdateTitle_Success_WhenEventIsInReadyStatus()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Ready)
                .Build();
            var newTitle = "Updated Title";

            // Act
            var result = newEvent.UpdateTitle(newTitle);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, newEvent.Title.Value);
            Assert.Equal(EventStatus.Draft, newEvent.Status);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenTitleIsEmpty()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();

            // Act
            var result = newEvent.UpdateTitle("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidTitle", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenTitleIsTooShort()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();

            // Act
            var result = newEvent.UpdateTitle("XY");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidTitleLength", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenTitleIsTooLong()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var longTitle = new string('A', 76);

            // Act
            var result = newEvent.UpdateTitle(longTitle);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidTitleLength", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenEventIsActive()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Active)
                .Build();

            // Act
            var result = newEvent.UpdateTitle("New Title");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateTitle_Fails_WhenEventIsCancelled()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Cancelled)
                .Build();

            // Act
            var result = newEvent.UpdateTitle("New Title");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }
    }
}